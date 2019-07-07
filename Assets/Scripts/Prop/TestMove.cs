using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TestMove : MonoBehaviour {
    public Rigidbody rigidbody;
    private float time = 2f;
    private float totalTime = 0;
    private float speed = 0.03f;
    [SerializeField]
    private HingeJoint m_HingeJoint;
	// Use this for initialization
	void Start () {
        //rigidbody.AddForce(Vector3.right*1000);
        //rigidbody.mo
        //rigidbody.DOMoveX(3, 4f).SetLoops(-1, LoopType.Yoyo);
        //m_HingeJoint.limits.min
    }
	
	// Update is called once per frame
	void Update () {
        //totalTime += Time.deltaTime;
        //if (totalTime <= time)
        //    rigidbody.MovePosition(rigidbody.position + Vector3.right * speed);
        //else if (totalTime > time && totalTime <= 2*time)
        //    rigidbody.MovePosition(rigidbody.position + Vector3.left * speed);
        //else
            //totalTime = 0;

    }
}
