using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{

    //returns int signifying winner
    //1 = attacker
    //0 = defender
    //-1 = error occurred
    public int fight(Hex attacker, Hex defender){

        //check that both attacker and defender exist
        if(attacker == null || defender == null){
            //reset visible sprites to non-selected variations
            if(attacker != null && attacker.getOwner() != null)
                attacker.setVisibleSprite(attacker.getOwner().getTile());
            
            if(defender != null && defender.getOwner() != null)
                defender.setVisibleSprite(defender.getOwner().getTile());
            
            return -1;
        }

        //check that attacker and defender are adjacent
        if(!attacker.isNeighbor(defender)){
            return -1;
        }
        
        int numAttackers = attacker.getUnits()-1;//leave one behind to defend
        int numDefenders = defender.getUnits();

        int winner = 0;
        string winnerString = "Defender";

        int attackerResult = 0;
        string attackerString = "";
        for(int i = 0; i < numAttackers; i++){
            int dieRoll = (int) Mathf.Floor(Random.Range(1f, 6f));
            attackerResult += dieRoll;
            attackerString += ""+dieRoll+",";
        }

        int defenderResult = 0;
        string defenderString = "";
        for(int i = 0; i < numDefenders; i++){
            int dieRoll = (int) Mathf.Floor(Random.Range(1f, 6f));
            defenderResult += dieRoll;
            defenderString += ""+dieRoll+",";
        }

        if(attackerResult > defenderResult){
            winner = 1;
            winnerString = "Attacker";
            defender.changeOwner(attacker.getOwner(), numAttackers);
        }

        attacker.subUnits(numAttackers);

        //Show results of battle
        Debug.Log(winnerString + " won the battle! "+attackerString+" atk to "+defenderString+" def with "+numAttackers+" attackers against "+numDefenders+" defenders.");

        return winner;

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
