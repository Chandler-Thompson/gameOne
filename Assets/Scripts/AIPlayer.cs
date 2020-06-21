using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Competitor
{
    
    public bool enableAI;

    // Start is called before the first frame update
    public override void Start()
    {
        if(enableAI)
            InvokeRepeating("takeTurn", 3.0f, 1.0f);
    }

    public void takeTurn(){
        
        Debug.Log("AI taking turn");

        //select random hex
        int hexIndex = (int) Mathf.Floor(Random.Range(0, territory.Count));
        Hex selectedHex = (Hex) territory[hexIndex];
        selectedHex.select();

        var coords = selectedHex.getCoords();

        //loop through selectedHex's neighbors to find a target
        for(int i = 0; i < MapManager.NUM_NEIGHBORS; i++){

            int nextX = coords.Item1 + MapManager.TILE_NEIGHBORS[i][0];
            int nextY = coords.Item2 + MapManager.TILE_NEIGHBORS[i][1];

            //check if hex exists
            if(mapManager.hexExists(nextX, nextY)){

                Hex neighbor = mapManager.getHex(nextX, nextY);

                //check if territory is enemy
                if(neighbor.getOwner() != this){
                    selectedHex.attack(neighbor);
                    return;
                }

            }

        }
            //single target found, attack!
    }

    // Update is called once per frame
    void Update()
    {
        


    }
}
