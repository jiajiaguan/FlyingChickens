using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropDisableAndEnable : MonoBehaviour {

    // Use this for initialization
    [SerializeField]
    private GameObject floorA, floorB;
    public float m_ShowTime = 3;
    public float m_DelayTimeB = 1;

    private int count = 0;
	void Start () {
    
       StartCoroutine(ShowDisabelOrEnable());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private IEnumerator ShowDisabelOrEnable(){
         while(true){
            floorA.SetActive(true);
            yield return new WaitForSeconds(m_DelayTimeB);
            floorB.SetActive(true);
            yield return new WaitForSeconds(m_ShowTime-m_DelayTimeB);
            floorA.SetActive(false);
            yield return new WaitForSeconds(m_DelayTimeB);
            floorA.SetActive(true);
            yield return new WaitForSeconds(m_ShowTime-m_DelayTimeB);
            floorB.SetActive(false);
        }
        // yield return new WaitForSeconds(m_DelayTimeB);
    }
    private void PropDisableOrEnableA(){
        count++;
        if(count%2==0){
            floorA.SetActive(true);
        }else{
            floorA.SetActive(false);
        }
    }
    float countB;
    private void PropDisableOrEnableB(){
         countB++;
        if(countB%2==0){
            floorB.SetActive(true);
        }else{
            floorB.SetActive(false);
        }
    }
}
