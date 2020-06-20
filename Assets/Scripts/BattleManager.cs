using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{

    public void fight(Hex attacker, Hex defender){

        int numAttackers = attacker.getUnits()-1;//leave one behind to defend
        int numDefenders = defender.getUnits();

        int attackerResult = 0;
        for(int i = 0; i < numAttackers; i++)
            attackerResult += (int) Mathf.Floor(Random.Range(1f, 6f));

        int defenderResult = 0;
        for(int i = 0; i < numDefenders; i++)
            defenderResult += (int) Mathf.Floor(Random.Range(1f, 6f));

        if(attackerResult > defenderResult)
            defender.changeOwner(attacker.getOwner(), numAttackers);

        attacker.subUnits(numAttackers);

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
