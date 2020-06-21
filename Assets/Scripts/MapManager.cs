using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapManager : MonoBehaviour
{

    public Hex baseHex;
    public Competitor[] players;

    public static int NUM_NEIGHBORS = 6;
    
    //tile's position to their neighbor based on where their neighbor is relative to them
    public static int[] POS_RELATIVE_TO_NEIGHBORS = new int[]{3, 4, 5, 0, 1, 2};
    public static int[][] TILE_NEIGHBORS = new int[][] {
        new int[]{ 0, -1},//top left
        new int[]{-1,  0},//mid left
        new int[]{-1,  1},//bot left
        new int[]{ 0,  1},//bot right
        new int[]{ 1,  0},//mid right
        new int[]{ 1, -1} //top right
    }; 

    private bool isMapDone;
    private Dictionary<(int, int), Hex> map = new Dictionary<(int, int), Hex>();

    //calculate hex grid placements
    private static float WIDTH = (Mathf.Sqrt(3) * 0.55f);//original: Mathf.Sqrt(3) * baseHex.hexSize
    private static float HEIGHT = (2 * 0.55f);//original: 2 * baseHex.hexSize
    private static float VERTICAL_DIST = (HEIGHT * 0.66f);//original: HEIGHT * 3.0f/4.0f
    private static float[][] HEX_PLACEMENTS = new float[][]{
        new float[]{-WIDTH/2,  VERTICAL_DIST},//top left
        new float[]{-WIDTH,    0.0f},         //mid left
        new float[]{-WIDTH/2, -VERTICAL_DIST},//bot left
        new float[]{ WIDTH/2, -VERTICAL_DIST},//bot right
        new float[]{ WIDTH,    0.0f},         //mid right
        new float[]{ WIDTH/2,  VERTICAL_DIST},//top right
    };

    /*
        Miscellaneous
    */
    public Competitor[] getPlayers(){
        return players;
    }

    /*
        Map Creation/Manipulation
    */

    public bool hexExists(int gridX, int gridY){
        Hex possibleHex = null;
        map.TryGetValue((gridX, gridY), out possibleHex);
        return possibleHex != null;
    }

    public Hex getHex(int gridX, int gridY){
        Hex result = null;
        map.TryGetValue((gridX, gridY), out result);

        return result;
    }

    //Will attempt to get float coords from any neighbors
    //creation failed and returns false if no neighbors found
    public Hex addHex(int gridX, int gridY){

        if(hexExists(gridX, gridY))//hex already created
            return null;

        var result = findNeighborCoords(gridX, gridY);
        if(result.Item1 != -1 && result.Item2 != -1 && result.Item3 != -1){

            float newX = result.Item1 + HEX_PLACEMENTS[result.Item3][0];
            float newY = result.Item2 + HEX_PLACEMENTS[result.Item3][1];

            Hex newHex = Instantiate(baseHex, new Vector3(newX, newY, 0), Quaternion.identity);
            newHex.initialize(gridX, gridY);
            connectNeighbors(newHex);
            map.Add((gridX, gridY), newHex);

            return newHex;
        }

        return null;
    }

    //Will attempt to get float coords from any neighbors
    //creation failed and returns false if no neighbors found
    public Hex addHex(Competitor owner, int gridX, int gridY){

        if(hexExists(gridX, gridY))//hex already created
            return null;

        var result = findNeighborCoords(gridX, gridY);
        if(result.Item1 != -1 && result.Item2 != -1 && result.Item3 != -1){

            float newX = result.Item1 + HEX_PLACEMENTS[result.Item3][0];
            float newY = result.Item2 + HEX_PLACEMENTS[result.Item3][1];

            Hex newHex = Instantiate(baseHex, new Vector3(newX, newY, 0), Quaternion.identity);
            newHex.initialize(owner, gridX, gridY);
            connectNeighbors(newHex);
            map.Add((gridX, gridY), newHex);

            return newHex;
        }

        return null;

    }

    public Hex addHex(int gridX, int gridY, float x, float y){

        if(!hexExists(gridX, gridY)){//hex not already created, so create it
            Hex newHex = Instantiate(baseHex, new Vector3(x, y, 0), Quaternion.identity);
            newHex.initialize(gridX, gridY);
            connectNeighbors(newHex);
            map.Add((gridX, gridY), newHex);
            return newHex;
        }

        return null;
    }

    public Hex addHex(Competitor owner, int gridX, int gridY, float x, float y){

        if(!hexExists(gridX, gridY)){//hex not already created, so create it
            Hex newHex = Instantiate(baseHex, new Vector3(x, y, 0), Quaternion.identity);
            newHex.initialize(owner, gridX, gridY);
            connectNeighbors(newHex);
            map.Add((gridX, gridY), newHex);
            return newHex;
        }

        return null;
    }

    //returns the transform position and neighbor position of the first 
    //neighbor found to the given grid coordinates
    //returns (-1, -1, -1) on nothing found
    private (float, float, int) findNeighborCoords(int gridX, int gridY){

        Hex neighbor = null;
        for(int i = 0; i < NUM_NEIGHBORS; i++){

            map.TryGetValue( (gridX + TILE_NEIGHBORS[i][0], gridY + TILE_NEIGHBORS[i][1]), out neighbor );

            if(neighbor != null){//neighbor found, get transform position
                Transform trans = neighbor.transform;
                return (trans.position.x, trans.position.y, i);
            }

        }

        return (-1.0f, -1.0f, -1);

    }

    //returns true if a neighbor found, false otherwise
    private bool connectNeighbors(Hex newHex){

        var coords = newHex.getCoords();
        Hex neighbor = null;

        bool neighborFound = false;

        int invNeighbor = 3;//keeps track of what neighbor position we are to our neighbor
        for(int i = 0; i < NUM_NEIGHBORS; i++){//loop through all potential positions

            map.TryGetValue((coords.Item1 + TILE_NEIGHBORS[i][0], coords.Item2 + TILE_NEIGHBORS[i][1]), out neighbor);

            if(neighbor != null){//neighbor found, add each other
                newHex.setNeighbor(neighbor, i);
                neighbor.setNeighbor(newHex, invNeighbor);
                neighborFound = true;
            }else{}

            invNeighbor++;
            if(invNeighbor > 5)
                invNeighbor = 0;

        }

        return neighborFound;

    }

    /*
        Map Generation
    */

    public bool isMapGenerated(){
        return isMapDone;
    }

    private void awardTerritory(Competitor player, Hex awardedTerritory){
        player.gainTerritory(awardedTerritory);
        awardedTerritory.setOwner(player);
    }

    public void beginGenerator(int mapType){ 

        //hold...hold...
        isMapDone = false;

        switch(mapType){
            case 0:
                generateHexShape(5);
                break;
            case 1:
                generateManualMap();
                break;
            default:
                generateManualMap();
                break;
        }

        //give all players this mapManager
        for(int i = 0; i < players.Length; i++)
            players[i].giveMapManager(this);

        //Let the games...begin!
        isMapDone = true;
    }

    private void generateManualMap(){
        addHex(0, 0, 0, 0);//place initial hex

        bool addedHumanPlayer = false;
        
        for(int i = 0; i < NUM_NEIGHBORS; i++){//build hexes around initial hex

            //create hex
            Hex createdHex = addHex(TILE_NEIGHBORS[i][0], TILE_NEIGHBORS[i][1]);

            Competitor player = null;
            if(i == NUM_NEIGHBORS - 1 && !addedHumanPlayer){//ensure human player is added at least once
                player = players[0];
            }else{
                int playerNum = (int) Mathf.Floor(UnityEngine.Random.Range(0f, players.Length));
                player = players[playerNum];
                addedHumanPlayer = (addedHumanPlayer || playerNum == 0);                
            }

            awardTerritory(player, createdHex);

        }

    }

    private void generateHexShape(int radius){
        generateHexShapeHelper(0, 0, 0, 0, radius);
    }

    private Hex generateHexShapeHelper(float x, float y, int gridX, int gridY, int maxDepth) {

        //check if at edge
        if(Hex.getDistance(gridX, gridY, 0, 0) >= maxDepth)
            return null;

        //check if hex made already
        if(hexExists(gridX, gridY)){
            Hex madeHex = getHex(gridX, gridY);
            return madeHex;
        }

        //choose a random owner
        int playerNum = (int) Mathf.Floor(UnityEngine.Random.Range(0, players.Length));
        Competitor player = players[playerNum];

        //if at center, make the owner the human player
        if(gridX == 0 && gridY == 0)
            player = players[0];

        //make this hex
        Hex newHex = Instantiate(baseHex, new Vector3(x, y, 0), Quaternion.identity);
        newHex.initialize(gridX, gridY);

        //assign hex to random owner
        awardTerritory(player, newHex);

        //add hex to the map for storage
        map.Add((gridX, gridY), newHex);

        //iterate through possible neighbors
        for(int i = 0; i < NUM_NEIGHBORS; i++){
            
            //create neighbor if it hasn't already been created
            int newGridX = gridX + TILE_NEIGHBORS[i][0];
            int newGridY = gridY + TILE_NEIGHBORS[i][1];
            float newX = x + HEX_PLACEMENTS[i][0];
            float newY = y + HEX_PLACEMENTS[i][1];

            Hex neighbor = null;

            if(!hexExists(newGridX, newGridY))//neighbor doesn't exist
                neighbor = generateHexShapeHelper(newX, newY, newGridX, newGridY, maxDepth);
            else//neighbor already exists
                neighbor = getHex(newGridX, newGridY);

            //add neighbors to each other, if neighbor isn't null
            if(neighbor != null){
                newHex.setNeighbor(neighbor, i);
                neighbor.setNeighbor(newHex, POS_RELATIVE_TO_NEIGHBORS[i]);
            }

        }

        //return made hex after making all neighbors and attaching them
        return newHex;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
