using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{

    //returns int signifying number of remaining units
    //>0 = attacking units remaining
    // 0 = error occurred
    //<0 = defending units remaining
    public int fight(Hex attacker, Hex defender){

        //check that both attacker and defender exist
        if(attacker == null || defender == null)          
            return 0;

        //check that attacker and defender are adjacent
        if(!attacker.isNeighbor(defender)){
            return 0;
        }
        
        int numAttackers = attacker.getMobileUnits();
        int numDefenders = defender.getTotalUnits();

        int remainingForces = numDefenders;
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
            remainingForces = numAttackers;
            winnerString = "Attacker";
        }

        //Show results of battle
        Debug.Log(winnerString + " won the battle! "+attackerString+" atk to "+defenderString+" def with "+numAttackers+" attackers against "+numDefenders+" defenders.");

        return remainingForces;

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
