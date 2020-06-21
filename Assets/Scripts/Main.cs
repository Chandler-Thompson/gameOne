using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{

    public MapManager mapManager;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting generation...");
        mapManager.beginGenerator();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
