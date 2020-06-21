using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{

    public void fight(Hex attacker, Hex defender){

        if(attacker == null || defender == null){
            //reset visible sprites to non-selected variations
            if(attacker != null)
                attacker.setVisibleSprite(attacker.getOwner().getTile());
            
            if(defender != null)
                defender.setVisibleSprite(defender.getOwner().getTile());
            
            return;
        }

        int numAttackers = attacker.getUnits()-1;//leave one behind to defend
        int numDefenders = defender.getUnits();

        string winner = "Defender";

        int attackerResult = 0;
        for(int i = 0; i < numAttackers; i++)
            attackerResult += (int) Mathf.Floor(Random.Range(1f, 6f));

        int defenderResult = 0;
        for(int i = 0; i < numDefenders; i++)
            defenderResult += (int) Mathf.Floor(Random.Range(1f, 6f));

        if(attackerResult > defenderResult){
            winner = "Attacker";
            defender.changeOwner(attacker.getOwner(), numAttackers);
        }

        attacker.subUnits(numAttackers);

        //Show results of battle
        Debug.Log(winner + " won the battle! "+attackerResult+" atk to "+defenderResult+" def");

        //reset visible sprites to non-selected variations
        attacker.setVisibleSprite(attacker.getOwner().getTile());
        defender.setVisibleSprite(defender.getOwner().getTile());

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
