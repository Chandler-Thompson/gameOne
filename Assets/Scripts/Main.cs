using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{

    public MapManager mapManager;
    
    private Competitor[] players;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting generation...");
        mapManager.beginGenerator();
        players = mapManager.getPlayers();
    }

    // Update is called once per frame
    void Update()
    {

        if(mapManager.isMapGenerated()){//only check victory conditions if map is done being made

            if(players.Length >= 1 && players[0].territoryCount() == 0){//check if human player is still alive
                Debug.Log("Oh no! You lost! Better luck next time!");
                SceneManager.LoadScene(0);//main menu
            }

            if(isAllDead()){
                Debug.Log("Congrats! you won!");
                SceneManager.LoadScene(0);//main menu
            }        

        }

    }

    private bool isAllDead(){

        if(players.Length == 0)
            return false;

        for(int i = 1; i < players.Length; i++){

            if(players[i].territoryCount() != 0)
                return false;

        }

        return true;
    }

}
