using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class PropRotate : MonoBehaviour {
    [SerializeField]
    private float time=1;
    [SerializeField,Range(1,89)]
    private float m_Range = 45;
    private Rigidbody m_Rigidbody;
	// Use this for initialization
	void Start () {
        //transform.DOLocalRotate(transform.up*300, 1).SetLoops(-1);
        transform.localEulerAngles = transform.right * -m_Range;
        transform.DOLocalRotate(transform.right * m_Range, time).SetLoops(-1,LoopType.Yoyo);
        m_Rigidbody = GetComponent<Rigidbody>();
        var _parent = GameObject.Find("GameScenceManger").transform;
        var _vec = _parent.TransformPoint( transform.right * m_Range);
        m_Rigidbody.DORotate(_vec, time).SetLoops(-1, LoopType.Yoyo);
    }
	
	// Update is called once per frame
	void Update () {
       

    }
}
