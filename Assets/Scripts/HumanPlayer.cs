using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Competitor
{

    private Hex selectedTile = null;

    public void setSelected(Hex hex){
        this.selectedTile = hex;
    }

    public Hex getSelected(){
        return this.selectedTile;
    }

}
