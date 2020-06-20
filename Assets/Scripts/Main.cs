using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{

    public MapGenerator mapGenerator;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting generation...");
        mapGenerator.beginGenerator();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
