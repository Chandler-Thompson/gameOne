using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Competitor : MonoBehaviour
{

    public Sprite compsTile;
    public Sprite compsTileSelected;

    protected Hex selectedTile;
    protected MapManager mapManager;
    protected ArrayList territory = new ArrayList();

    public void giveMapManager(MapManager mapManager){
        this.mapManager = mapManager;
    }

    public Hex getSelected(){
        return this.selectedTile;
    }

    public void setSelected(Hex hex){
        this.selectedTile = hex;
    }

    public Sprite getTile(){
        return this.compsTile;
    }

    public Sprite getSelectedTile(){
        return this.compsTileSelected;
    }

    public void gainTerritory(Hex hex){
        var coords = hex.getCoords();
        territory.Add(hex);
    }

    public void loseTerritory(Hex hex){
        territory.Remove(hex);
    }

    public bool hasTerritory(Hex hex){
        return territory.Contains(hex);
    }

    public int territoryCount(){
        return territory.Count;
    }

    // override object.Equals
    public override bool Equals(object obj)
    {
        //
        // See the full list of guidelines at
        //   http://go.microsoft.com/fwlink/?LinkID=85237
        // and also the guidance for operator== at
        //   http://go.microsoft.com/fwlink/?LinkId=85238
        //
        
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        
        Competitor other = (Competitor) obj;

        return compsTile.Equals(other.getTile());
    }
    
    // override object.GetHashCode
    public override int GetHashCode()
    {
        // TODO: write your implementation of GetHashCode() here
        throw new System.NotImplementedException();
        //return base.GetHashCode();
    }

    public virtual void Start(){
        Debug.Log("Competitor start.");
    }

    void Update(){
    }

}
