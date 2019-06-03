
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PropMovePosition : MonoBehaviour {
    [SerializeField]
    private Vector3 m_EndPos;
    [SerializeField]
    private Vector3 m_StartPos = Vector3.zero;
    [SerializeField]
    private float m_Duration = 0.8f;
    [SerializeField]
    private float m_DelayTime=0;
    private Vector3 startPos;
    private Rigidbody m_rigibody;
    private Transform parent;
	// Use this for initialization
	void Start () {
        if (m_StartPos != Vector3.zero)
            transform.localPosition = m_StartPos;
        m_rigibody = GetComponent<Rigidbody>();
        parent = GameObject.Find("GameScenceManger").transform;
        // transform.DOLocalMove(m_EndPos, m_Duration).SetLoops(-1,LoopType.Yoyo).SetDelay(m_DelayTime);
        // m_rigibody = transform.Getcomm
        m_EndPos = parent.TransformPoint(m_EndPos);
        m_rigibody.DOMove(m_EndPos, m_Duration).SetLoops(-1,LoopType.Yoyo).SetDelay(m_DelayTime);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
