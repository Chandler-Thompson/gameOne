using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCounter : MonoBehaviour
{

    private GameObject grandparent = null;
    private TMPro.TextMeshProUGUI textMesh = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void updateCounter(){

        if(grandparent == null)//need to set grandparent
            grandparent = gameObject.transform.parent.transform.parent.gameObject;

        if(textMesh == null)//need to get textMesh
            textMesh = gameObject.GetComponent<TMPro.TextMeshProUGUI>();


        int count = grandparent.GetComponent<Hex>().getTotalUnits();
        textMesh.text = ""+count;

    }

    // Update is called once per frame
    void Update()
    {
        
        if(grandparent == null)//need to set grandparent
            grandparent = gameObject.transform.parent.transform.parent.gameObject;

        if(textMesh == null)//need to get textMesh
            textMesh = gameObject.GetComponent<TMPro.TextMeshProUGUI>();


        int count = grandparent.GetComponent<Hex>().getTotalUnits();
        textMesh.text = ""+count;

    }
}
