using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapGenerator : MonoBehaviour
{

    public Hex baseHex;
    public int mapRadius = 1;
    public Hex[] map;

    private Dictionary<(int, int), Hex> hexTiles = new Dictionary<(int, int), Hex>();

    public void beginGenerator(){ 

        //Debug/Testing statements
        // Hex hex0 = Instantiate(baseHex, new Vector3(0, 0, 0), Quaternion.identity);
        // Hex hex1 = Instantiate(baseHex, new Vector3(0, 0, 0), Quaternion.identity);
        // Hex hex2 = Instantiate(baseHex, new Vector3(0, 0, 0), Quaternion.identity);
        // Hex hex3 = Instantiate(baseHex, new Vector3(0, 0, 0), Quaternion.identity);
        // Hex hex4 = Instantiate(baseHex, new Vector3(0, 0, 0), Quaternion.identity);
        // Hex hex5 = Instantiate(baseHex, new Vector3(0, 0, 0), Quaternion.identity);
        // Hex hex6 = Instantiate(baseHex, new Vector3(0, 0, 0), Quaternion.identity);
        
        // Hex hex7 = Instantiate(baseHex, new Vector3(0, 0, 0), Quaternion.identity);
        // Hex hex8 = Instantiate(baseHex, new Vector3(0, 0, 0), Quaternion.identity);

        // hex0.initialize(0, 0);
        // hex1.initialize(0, -1);
        // hex2.initialize(-1, 0);
        // hex3.initialize(-1, 1);
        // hex4.initialize(0, 1);
        // hex5.initialize(1, 0);
        // hex6.initialize(1, -1);

        // hex7.initialize(-1, -2);
        // hex8.initialize(0, 3);

        // Debug.Log("Dist: "+hex7.getDistance(hex0));
        // Debug.Log("Dist: "+hex8.getDistance(hex0));

        // Debug.Log("Dist: "+hex7.getDistance(0, 0));
        // Debug.Log("Dist: "+hex8.getDistance(0, 0));

        // Destroy(hex0);
        // Destroy(hex1);
        // Destroy(hex2);
        // Destroy(hex3);
        // Destroy(hex4);
        // Destroy(hex5);
        // Destroy(hex6);

        // Destroy(hex7);
        // Destroy(hex8);

        generateHexShape();

    }

    public void generateHexShape(){
        HashSet<(int, int)> createdTiles = new HashSet<(int, int)>();

        //calculate hex grid placements
        float width = (Mathf.Sqrt(3) * baseHex.hexSize);
        float height = (2 * baseHex.hexSize);

        float verticalDist = (float) (height * 3.0/4.0);

        int[][] tileChildren = new int[][] {
            new int[]{-1,  1},
            new int[]{-1,  0},
            new int[]{-1, -1},
            new int[]{ 1, -1},
            new int[]{ 1,  0},
            new int[]{ 1,  1}
        }; 
        float[][] transitions = new float[][]{
            new float[]{-width/2,  verticalDist},//top left
            new float[]{-width,    0.0f},        //mid left
            new float[]{-width/2, -verticalDist},//bot left
            new float[]{ width/2, -verticalDist},//bot right
            new float[]{ width,    0.0f},        //mid right
            new float[]{ width/2,  verticalDist},//top right
        };
        generateHex(new HashSet<(int, int)>(), tileChildren, transitions, 0, 0, 0, 0, mapRadius);
    }

    private Hex generateHex(HashSet<(int, int)> created, int[][] tileChildren, float[][] tileTransitions, float x, float y, int gridX, int gridY, int maxDepth) {

        //create this hex
        if( Hex.getDistance(0, 0, gridX, gridY) >= maxDepth ){
            Debug.Log("\tReached edge of shape at "+maxDepth+" with ("+gridX+","+gridY+")");
            return null;
        }

        if(created.Contains( (gridX, gridY) )){
            Debug.Log("\tAlready generated hex at .");
            Hex result = null;
            hexTiles.TryGetValue( (gridX, gridY), out result);
            return result;
        }

        Debug.Log("Instantiating ("+gridX+","+gridY+") at ("+x+","+y+")");

        //create this hex and add it to the created set
        Hex thisHex = Instantiate(baseHex, new Vector3(x, y, 0), Quaternion.identity);
        thisHex.initialize(gridX, gridY);
        created.Add((gridX, gridY));

        //add neighbors to dict of hex tiles and as neighbors to this hex
        for(int i = 0; i < 6; i++){//iterate through all sides/neighbors

            float nextX = x+tileTransitions[i][0];
            float nextY = y+tileTransitions[i][1];
            int nextGridX = gridX+tileChildren[i][0];
            int nextGridY = gridY+tileChildren[i][1];

            // Debug.Log("Creating Hex at ("+nextGridX+","+nextGridY+") ("+nextX+","+nextY+")");

            Debug.Log("Looking at neighbor "+i);

            //recursively create neigbor
            Hex neighborHex = generateHex(created, tileChildren, tileTransitions, nextX, nextY, nextGridX, nextGridY, maxDepth);

            if(neighborHex is object){

                try{
                    hexTiles.Add( (nextGridX, nextGridY), neighborHex );
                }catch(ArgumentException e){
                    //We have already created the tile, just need to add it as a neighbor
                }
                
                thisHex.setNeighbor(neighborHex, i);
            }

        }
        
        return thisHex;
    }

    public Hex getHex(int x, int y){
        Hex result = null;
        hexTiles.TryGetValue((x, y), out result);

        return result;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
