using UnityEngine;
using System.Collections;
using System;

public class PlayerMove : MonoBehaviour {

    public float speed = 1f;
    private Animator anim;
    private int groundLayerIndex = -1;
    private float move_speed=1;
    private Rigidbody M_rigidbody;
    private float turn_speed = 0.5f;
    private float vertical=0;
    private float horizonta=0;
    private bool isMoving;
    private Transform m_transform;
    private bool isGround = true;
    private float jumpSpeed = 300;

    // Use this for initialization
    void Start () {
        anim = this.GetComponent<Animator>();
        groundLayerIndex = LayerMask.GetMask("Ground");
        M_rigidbody = GetComponent<Rigidbody>();
        m_transform = transform;

    }
	
	// Update is called once per frame
	void FixedUpdate() {
       
        if (isMoving && (vertical != 0 || horizonta != 0))
        {
            //Rotation(v, h);
            //anim.SetBool("stand_walk", true);
            //anim.SetBool("walk_stand", false);
            //anim.SetBool("Move", true);
            //transform.Translate(Vector3.forward * move_speed * Time.deltaTime);
            //Debug.Log("m_transform.position.y: " + m_transform.position.y + "startJumpY: " + startJumpY);
            M_rigidbody.MovePosition(m_transform.position + new Vector3(horizonta, 0, vertical) * speed * Time.deltaTime);
            //this.transform.
            transform.LookAt(new Vector3(m_transform.position.x + horizonta, m_transform.position.y, m_transform.position.z + vertical));
        }
        else
        {
            //anim.SetBool("Move", false);
        }
        if(Input.GetKeyDown(KeyCode.Space)){
            PlayerJump();
        }
        if(m_transform.position.y - startJumpY>=limitDetalY){
            //m_transform.position = new Vector3(m_transform.position.x, startJumpY + limitDetalY, m_transform.position.z) ;
            isJump = true;
        }else {
            isJump = false;
        }
    }
    // 碰撞开始
    private bool isJump = false;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Floor")//碰撞的是Plane  
        {
            isJump = false;
        }
    }



    public void playerMove(float h, float v)
    {
        vertical = v;
        horizonta = h;
    }
    private float limitDetalY = 3;
    private float startJumpY = 0;
    public void PlayerJump(){
        //startJumpY = m_transform.position.y;
        if(!isJump){
            M_rigidbody.AddForce(Vector3.up*jumpSpeed);
            //isJump = true;
        }
        //Debug.Log("PlayerJump*******"+(Vector3.up*jumpSpeed));
    }

    public void IsStartMove(bool isStart){
        isMoving = isStart;
    }
    private void Rotation(float vertical, float horizontal)
    {
        Vector3 targeDirection = new Vector3(horizontal, 0f, vertical);
        Quaternion targetRotation = Quaternion.LookRotation(targeDirection, Vector3.up);
        Quaternion newRotation = Quaternion.Lerp(M_rigidbody.rotation, targetRotation, turn_speed * Time.deltaTime);
        transform.rotation = newRotation;
     
    }
}
