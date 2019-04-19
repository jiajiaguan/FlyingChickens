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
    private float m_Vertical = 0;
    private float m_Horizonta = 0;
    private bool isMoving;
    private Transform m_transform;
    private bool isGround = true;
    public float jumpSpeed = 100;
    private bool isPlaying = false;

    public System.Action<GameController.GameState> OnChangeGameState;

    //DataConfig
    public float curHealth = 3;
    // Use this for initialization
    void Start () {
        anim = this.GetComponentInChildren<Animator>();
        groundLayerIndex = LayerMask.GetMask("Ground");
        M_rigidbody = GetComponentInChildren<Rigidbody>();
        m_transform = transform;

    }

    // Update is called once per frame

    //private void Update()
    //{
    //    ControllMove();
    //}

	
	// Update is called once per frame
	void Update() {
        //if(isPlaying){
        if (!isPlaying)
            return;
            ControllMove();
            if (isMoving && (vertical != 0 || horizonta != 0))
            {
                //Rotation(v, h);
                anim.Play("run_b");
                // anim.SetBool("run_b", true);
                // anim.SetBool("standby", false);
                // anim.SetBool("jump", false);
                //transform.Translate(Vector3.forward * move_speed * Time.deltaTime);
                M_rigidbody.MovePosition(m_transform.position + new Vector3(horizonta, 0, vertical) * speed * Time.deltaTime);
                //this.transform.
                transform.LookAt(new Vector3(m_transform.position.x + horizonta, m_transform.position.y, m_transform.position.z + vertical));
            }
            else if(!isJump)
            {
                // anim.SetBool("run_b", false);
                anim.Play("standby");
            }
            if(Input.GetKeyDown(KeyCode.Space)){
                PlayerJump();
            }
            
            //if(m_transform.position.y - startJumpY>=limitDetalY){
            //    //m_transform.position = new Vector3(m_transform.position.x, startJumpY + limitDetalY, m_transform.position.z) ;
            //    isJump = true;
            //}else {
            //    isJump = false;
            //}
        //}

        //游戏结束判断1、人物位置
        if(transform.localPosition.y < -1){
            Debug.LogError("Game Over");
            isPlaying = false;
            if (OnChangeGameState != null){
                if (curHealth > 0)
                    OnChangeGameState(GameController.GameState.GameOver);
                else
                    OnChangeGameState(GameController.GameState.Fall);
            }
            M_rigidbody.useGravity = false;
        }
    }
    // 碰撞开始
    private bool isJump = false;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))//碰撞的是Plane  
        {
            isJump = false;
        }
    }
    //开始游戏
    public void StartNewGame(Vector3 bornPos){
        isPlaying = true;
        curHealth--;
        gameObject.SetActive(false);
        transform.position = bornPos;
        gameObject.SetActive(true);
        M_rigidbody.useGravity = true;
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
            // anim.SetBool("jump", true);
            anim.Play("jump");
            isJump = true;
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

    private void ControllMove(){
        bool m_IsMoving = false;
        m_Vertical = 0;
        m_Horizonta = 0;
        if (Input.GetKey(KeyCode.W)){
            m_IsMoving = true;
            //playerMove(horizonta,1);
            m_Vertical = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            m_IsMoving = true;
            //playerMove(-1, vertical);
            m_Horizonta = -1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            m_IsMoving = true;
            //playerMove(horizonta, -1);
            m_Vertical = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            m_IsMoving = true;
            //playerMove(1, vertical);
            m_Horizonta = 1;
        }
        if(m_IsMoving){
            isMoving = true;
            playerMove(m_Horizonta, m_Vertical);
        }
        if(Input.GetKeyUp(KeyCode.W)|| Input.GetKeyUp(KeyCode.A)|| Input.GetKeyUp(KeyCode.S)|| Input.GetKeyUp(KeyCode.D)){
            isMoving = false;
        }
    }


}
