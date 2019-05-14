using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script requires you to have setup your animator with 3 parameters, "InputMagnitude", "InputX", "InputZ"
//With a blend tree to control the inputmagnitude and allow blending between animations.
public class GameBGMove : MonoBehaviour
{
    public float speed;  

    private float movespeed;  


    public float minPositionX;  

    public float terPositionX;  

// Use this for initialization

void Start () {  


}  



// Update is called once per frame

void Update () {  

      movespeed = speed * Time.deltaTime;  

      transform.Translate(Vector3.left * movespeed, Space.World); //向左移動

if (transform.localPosition.x <= minPositionX)  

      {  

          transform.localPosition = new Vector3(terPositionX+(transform.localPosition.x-minPositionX), transform.localPosition.y,transform.localPosition.z);  

      }  

}  
    
}

