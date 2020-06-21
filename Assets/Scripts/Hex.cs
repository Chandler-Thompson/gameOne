using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hex : MonoBehaviour
{

    public float hexSize = 1;
    public Sprite defaultSprite;
    public HumanPlayer humanPlayer;
    public AIPlayer[] aiPlayers;
    public BattleManager battleManager;
    public Hex[] neighbors;

    //grid coords
    private int x;
    private int y;
    private int z;

    private Sprite visibleSprite;
    public Competitor owner;
    private int numUnits;

    // Start is called before the first frame update
    void Start()
    {
        numUnits = 0;

        if(owner != null)
            visibleSprite = owner.getTile();
        else
            visibleSprite = defaultSprite;

        SpriteRenderer renderer = null;
        gameObject.TryGetComponent<SpriteRenderer>(out renderer);

        renderer.sprite = visibleSprite;

        InvokeRepeating("reinforce", 0.0f, 3.0f);
    }

    public void initialize(int x, int y){
        this.x = x;
        this.y = y;
        this.z = -(x + y);
        neighbors = new Hex[6];
    }

    public void initialize(Competitor owner, int x, int y){
        this.x = x;
        this.y = y;
        this.z = -(x + y);
        neighbors = new Hex[6];
        this.owner = owner;
    }

    public (int, int, int) getCoords(){
        return (x, y, z);
    }

    public bool isNeighbor(Hex other){
        for(int i = 0; i < neighbors.Length; i++){

            if(neighbors[i] != null && other != null && neighbors[i].Equals(other))
                return true;
        }
        return false;
    }

    public void setNeighbor(Hex neighbor, int position){
        neighbors[position] = neighbor;    
    }

    public void setVisibleSprite(Sprite visibleSprite){
        this.visibleSprite = visibleSprite;
        SpriteRenderer renderer = null;
        gameObject.TryGetComponent<SpriteRenderer>(out renderer);
        renderer.sprite = visibleSprite;
    }

    public Hex getNeighbor(int position){

        switch(position){

            case 0://top left
                return neighbors[0];
            case 1://mid left
                return neighbors[1];
            case 2://bot left
                return neighbors[2];
            case 3://bot right
                return neighbors[3];
            case 4://mid right
                return neighbors[4];
            case 5://top right
                return neighbors[5];
            default:
                return null;
        }
    }

    public static int getDistance(int x1, int y1, int x2, int y2){
        int z1 = -(x1 + y1);
        int z2 = -(x2 + y2);
        return Mathf.Max( Mathf.Abs(x1 - x2), Mathf.Abs(y1 - y2), Mathf.Abs(z1 - z2) );        
    }

    public int getDistance(int x, int y){
        int z = -(x + y);
        return Mathf.Max( Mathf.Abs(this.x - x), Mathf.Abs(this.y - y), Mathf.Abs(this.z - z));
    }

    public int getDistance(Hex other){
        //Get the max of x, y, and z to which correlates to the distance
        return Mathf.Max( Mathf.Abs(this.x - other.x), Mathf.Abs(this.y - other.y), Mathf.Abs(this.z - other.z));
    }

    public Competitor getOwner(){
        return this.owner;
    }

    public void setOwner(Competitor owner){
        this.owner = owner;
        setVisibleSprite(owner.getTile());
    }

    public int getTotalUnits(){
        return this.numUnits;
    }

    public int getMobileUnits(){
        return this.numUnits-1;
    }

    public void subUnits(int amount){
        //automatically handle potential forgetfulness on passing in a negative
        this.numUnits = (amount < 0) ? numUnits+amount : numUnits-amount;
    }

    public void addUnits(int amount){
        //automatically handle potential forgetfullness on passing in a negative
        this.numUnits = (amount > 0) ? numUnits+amount : numUnits-amount;
    }

    public void subAll(){
        this.numUnits = 1;
    }

    public void changeOwner(Competitor owner, int numUnits){

        if(this.owner != null)//old owner loses this territory
            this.owner.loseTerritory(this);        

        //change owners, accept new garrison, and fly new flag
        this.owner = owner;
        this.numUnits = numUnits;
        this.setVisibleSprite(owner.getTile());

        //new owner gains this territory
        this.owner.gainTerritory(this);
    }

    //have human player select this hex
    public void select(){

        //make sure other tile is deselected first
        if(owner.getSelected() != null)
            owner.getSelected().deselect();

        //select this tile
        owner.setSelected(this);
        this.setVisibleSprite(owner.getSelectedTile());
    }

    public void deselect(){
        
        //deslect this
        if(this.getOwner().getSelected() == this)
            owner.setSelected(null);

        //visually show deselection
        this.setVisibleSprite(owner.getTile());
    }

    void OnMouseDown(){

        //If the owner of this tile is the human player
        if(owner != null && owner.Equals(humanPlayer)){
            Debug.Log("Selected hex has "+this.getTotalUnits()+" units.");

            //and the human player already selected a tile
            if(humanPlayer.getSelected() != null){

                if(humanPlayer.getSelected() == this){//selected tile is this one, deselect it
                    deselect();
                }else{//this is a different tile with the same owner, transfer units to here if adjacent, and deselect both

                    //transfer units, if adjacent
                    if(this.isNeighbor(humanPlayer.getSelected())){
                        Hex other = humanPlayer.getSelected();
                        int numTransfer = other.getMobileUnits();
                        this.addUnits(numTransfer);
                        other.subUnits(numTransfer);

                        //deselect both tiles
                        deselect();
                        humanPlayer.getSelected().deselect();

                    }else{//make this tile the selected one
                        select();
                    }
                }
            }else{//no tile selected, so select this one
                select();
            }
            
        }else if(humanPlayer.getSelected() != null){//Owner of the tile is an AI, or it is unowned

            if(this.isNeighbor(humanPlayer.getSelected())){//if the two tiles are neighbors
                Hex.fight(humanPlayer.getSelected(), this);//then fight
            }
        }
    }
    
    public int attack(Hex other){
        return fight(this, other);
    }

    public int defend(Hex other){
        return fight(other, this);
    }

    public static int fight(Hex attacker, Hex defender){

        //commence battle
        int remainingForces = BattleManager.fight(attacker, defender);

        //remove attackers from attacking tile
        attacker.subAll();

        //if attacker won, change ownership of defending tile
        if(remainingForces > 0){
            defender.changeOwner(attacker.getOwner(), remainingForces);
        }else if(remainingForces < 0){//defender won

        }

        //deselect tile
        attacker.deselect();

        return remainingForces;

    }

    public void reinforce(){
        if(owner != null)
            numUnits++;
    }

    // Update is called once per frame
    void Update(){}
}
