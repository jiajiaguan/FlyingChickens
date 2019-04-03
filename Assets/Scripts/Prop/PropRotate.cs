using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class PropRotate : MonoBehaviour {
    [SerializeField]
    private Transform target;
	// Use this for initialization
	void Start () {
        //transform.DOLocalRotate(transform.up*300, 1).SetLoops(-1);

	}
	
	// Update is called once per frame
	void Update () {
        //transform.DOMoveX(100, 1);
        //Tween tween = DOTween.To(() => transform.position, r => transform.position = r, new Vector3(5, 5, 5), 1);
        //transform.Rotate(target.transform.up*Time.deltaTime*100,Space.Self);
        transform.Rotate(transform.up*Time.deltaTime*100,Space.World);

    }
}
