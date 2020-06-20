using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hex : MonoBehaviour
{

    public float hexSize = 1;
    public Sprite defaultSprite;

    private Hex[] neighbors;
    private int x;
    private int y;
    private int z;

    private Sprite visibleSprite;
    private Competitor owner;
    private int numUnits;
    private int updateCounter = 0;

    // Start is called before the first frame update
    void Start()
    {

        SpriteRenderer renderer = null;
        gameObject.TryGetComponent<SpriteRenderer>(out renderer);

        renderer.sprite = defaultSprite;

        owner = null;
        numUnits = 0;
        visibleSprite = defaultSprite;
    }

    public Hex hexFactory(int x, int y){
        Hex createdHex = new Hex();
        createdHex.initialize(x, y);
        return createdHex;
    }

    public void initialize(int x, int y){
        this.x = x;
        this.y = y;
        this.z = -(x + y);
        neighbors = new Hex[6];
    }

    public (int, int, int) getGridCoords(){
        return (x, y, z);
    }

    public void setNeighbor(Hex neighbor, int position){
        neighbors[position] = neighbor;    
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

    public int getUnits(){
        return this.numUnits;
    }

    public void subUnits(int amount){
        this.numUnits -= amount;
    }

    public void changeOwner(Competitor owner, int numUnits){
        this.owner = owner;
        this.numUnits = numUnits;
        visibleSprite = owner.getTile();
    }

    void onMouseDown(){
        Debug.Log(name + " was clicked.");
    }

    // Update is called once per frame
    void Update()
    {
        
        // //spawn more units
        // if(updateCounter == 300){
        //     numUnits++;
        //     updateCounter = 0;
        // }else
        //     updateCounter++;

    }
}
