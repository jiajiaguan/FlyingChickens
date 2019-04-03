using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropDisableAndEnable : MonoBehaviour {

    // Use this for initialization
    [SerializeField]
    private GameObject floorA, floorB;

    private int count = 0;
	void Start () {
        InvokeRepeating("PropDisableOrEnable", 1, 3);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void PropDisableOrEnable(){
        count++;
        if(count%2==0){
            floorA.SetActive(true);
            floorB.SetActive(false);
        }else{
            floorA.SetActive(false);
            floorB.SetActive(true);
        }
    }
}
