using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapManager : MonoBehaviour
{

    public Hex baseHex;
    public int mapRadius = 1;

    private static int NUM_NEIGHBORS = 6;
    private Dictionary<(int, int), Hex> map = new Dictionary<(int, int), Hex>();
    private static int[][] tileNeighbors = new int[][] {
        new int[]{-1,  1},
        new int[]{-1,  0},
        new int[]{-1, -1},
        new int[]{ 1, -1},
        new int[]{ 1,  0},
        new int[]{ 1,  1}
    }; 

    /*
        Map Creation/Manipulation
    */

    public bool hexExists(int gridX, int gridY){
        Hex possibleHex = null;
        map.TryGetValue((gridX, gridY), out possibleHex);
        return possibleHex == null;
    }

    public Hex getHex(int gridX, int gridY){
        Hex result = null;
        map.TryGetValue((gridX, gridY), out result);

        return result;
    }

    //Will attempt to get float coords from any neighbors
    //creation failed and returns false if no neighbors found
    public bool addHex(int gridX, int gridY){

        if(hexExists(gridX, gridY))//hex already created
            return false;

        var result = findNeighborCoords(gridX, gridY);
        if(result.Item1 != -1 && result.Item2 != -1 && result.Item3 != -1){

            float newX = result.Item1 + tileNeighbors[result.Item3][0];
            float newY = result.Item2 + tileNeighbors[result.Item3][1];

            Hex newHex = Instantiate(baseHex, new Vector3(newX, newY, 0), Quaternion.identity);
            connectNeighbors(newHex);
            map.Add((gridX, gridY), newHex);

            return true;
        }

        return false;
    }

    public void addHex(int gridX, int gridY, float x, float y){

        if(!hexExists(gridX, gridY)){//hex not already created, so create it
            Hex newHex = Instantiate(baseHex, new Vector3(x, y, 0), Quaternion.identity);
            newHex.initialize(gridX, gridY);
            connectNeighbors(newHex);
            map.Add((gridX, gridY), newHex);
        }
    }

    public void addHex(Competitor owner, int gridX, int gridY, float x, float y){

        if(!hexExists(gridX, gridY)){//hex not already created, so create it
            Hex newHex = Instantiate(baseHex, new Vector3(x, y, 0), Quaternion.identity);
            newHex.initialize(owner, gridX, gridY);
            connectNeighbors(newHex);
            map.Add((gridX, gridY), newHex);
        }
    }

    //returns the transform position and neighbor position of the first 
    //neighbor found to the given grid coordinates
    //returns (-1, -1, -1) on nothing found
    private (float, float, int) findNeighborCoords(int gridX, int gridY){

        Hex neighbor = null;
        for(int i = 0; i < NUM_NEIGHBORS; i++){

            map.TryGetValue( (gridX + tileNeighbors[i][0], gridY + tileNeighbors[i][1]), out neighbor );

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
        for(int i = 0; i < NUM_NEIGHBORS; i++){

            map.TryGetValue((coords.Item1 + tileNeighbors[i][0], coords.Item2 + tileNeighbors[i][1]), out neighbor);

            if(neighbor != null){//neighbor found, add each other
                newHex.setNeighbor(neighbor, i);
                neighbor.setNeighbor(newHex, invNeighbor);
                neighborFound = true;
            }

            invNeighbor++;
            if(invNeighbor > 5)
                invNeighbor = 0;

        }

        return neighborFound;

    }

    /*
        Map Generation
    */

    public void beginGenerator(){ 
        //generateHexShape();
        generateManualMap();
    }

    public void generateManualMap(){

        addHex(0, 0, 0, 0);//place initial hex
        
        for(int i = 0; i < NUM_NEIGHBORS; i++){//build hexes around initial hex
            addHex(tileNeighbors[i][0], tileNeighbors[i][1]);
        }

    }

    public void generateHexShape(){
        HashSet<(int, int)> createdTiles = new HashSet<(int, int)>();

        //calculate hex grid placements
        float width = (Mathf.Sqrt(3) * baseHex.hexSize);
        float height = (2 * baseHex.hexSize);

        float verticalDist = (float) (height * 3.0/4.0);

        float[][] transitions = new float[][]{
            new float[]{-width/2,  verticalDist},//top left
            new float[]{-width,    0.0f},        //mid left
            new float[]{-width/2, -verticalDist},//bot left
            new float[]{ width/2, -verticalDist},//bot right
            new float[]{ width,    0.0f},        //mid right
            new float[]{ width/2,  verticalDist},//top right
        };
        generateHexShapeHelper(new HashSet<(int, int)>(), transitions, 0, 0, 0, 0, mapRadius);
    }

    private Hex generateHexShapeHelper(HashSet<(int, int)> created, float[][] tileTransitions, float x, float y, int gridX, int gridY, int maxDepth) {

        //check hex distance from center
        if( Hex.getDistance(0, 0, gridX, gridY) >= maxDepth ){
            Debug.Log("\tReached edge of shape at "+maxDepth+" with ("+gridX+","+gridY+")");
            return null;
        }

        //check hex hasn't been created
        if(created.Contains( (gridX, gridY) )){
            Debug.Log("\tAlready generated hex at .");
            Hex result = null;
            map.TryGetValue( (gridX, gridY), out result);
            return result;
        }

        Debug.Log("Instantiating ("+gridX+","+gridY+") at ("+x+","+y+")");

        //create this hex and add it to the created set
        Hex thisHex = Instantiate(baseHex, new Vector3(x, y, 0), Quaternion.identity);
        thisHex.initialize(gridX, gridY);
        created.Add((gridX, gridY));

        //add neighbors to dict of hex tiles and as neighbors to this hex
        for(int i = 0; i < NUM_NEIGHBORS; i++){//iterate through all sides/neighbors

            float nextX = x+tileTransitions[i][0];
            float nextY = y+tileTransitions[i][1];
            int nextGridX = gridX+tileNeighbors[i][0];
            int nextGridY = gridY+tileNeighbors[i][1];

            Debug.Log("Looking at neighbor "+i);

            //recursively create neigbor
            Hex neighborHex = generateHexShapeHelper(created, tileTransitions, nextX, nextY, nextGridX, nextGridY, maxDepth);

            //if neighbor not null
            if(neighborHex is object){

                try{//add neighbor to map
                    map.Add( (nextGridX, nextGridY), neighborHex );
                }catch(ArgumentException){
                    //We have already created the tile, just need to add it as a neighbor
                }
                
                //add neighbor as neighbor
                thisHex.setNeighbor(neighborHex, i);
            }

        }
        
        return thisHex;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
