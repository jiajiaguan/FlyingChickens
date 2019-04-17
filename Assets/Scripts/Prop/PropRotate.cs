using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class PropRotate : MonoBehaviour {
    [SerializeField]
    private float time=1;
    [SerializeField]
    private float m_Range = 300;
	// Use this for initialization
	void Start () {
        //transform.DOLocalRotate(transform.up*300, 1).SetLoops(-1);
        transform.localEulerAngles = transform.right * -m_Range;
        transform.DOLocalRotate(transform.right * m_Range, 1).SetLoops(-1,LoopType.Yoyo);
	}
	
	// Update is called once per frame
	void Update () {
       

    }
}
