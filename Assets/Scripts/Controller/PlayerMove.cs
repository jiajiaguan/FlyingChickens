using System.Net.Mime;
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerMove : MonoBehaviour {
enum PlayerState{
    None,
    Standy,
    Move,
    Jump
}
    [SerializeField]
    private Transform m_hearthParent;
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
    public float curHealth{
        set{
            _curHealth = value;
            //m_hearthParent.text = value.ToString();
            for(int i = 0; i < 3; i++) { 
                if(i < _curHealth) {
                    m_hearthParent.GetChild(i).gameObject.SetActive(true);
                }else
                    m_hearthParent.GetChild(i).gameObject.SetActive(false);
            }
        }
        get{
            return _curHealth;
        }
    }
    private float _curHealth = 3;
    private PlayerState _curPlayerState = PlayerState.None;
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
                _curPlayerState = PlayerState.Move;
                
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
                _curPlayerState = PlayerState.Standy;
                // anim.Play("standby");
            }
            if(Input.GetKeyDown(KeyCode.Space)){
                PlayerJump();
            }
            UpdatePlayerState();
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
            vertical = 0 ;
            horizonta = 0;
            M_rigidbody.useGravity = false;
            if (OnChangeGameState != null){
                if (curHealth > 0)
                    OnChangeGameState(GameController.GameState.Fall);
                else
                    OnChangeGameState(GameController.GameState.GameOver);
            }
            // M_rigidbody.useGravity = false;
        }
    }
    // 碰撞开始
    private bool isJump = false;
    void OnCollisionEnter(Collision collision)
    {
        if (!isPlaying)
            return;
        //Debug.LogError("collision.gameObject.layer"+collision.gameObject.layer+"LayerMask.NameToLayer: "+LayerMask.NameToLayer("Floor"));
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))//碰撞的是Plane  
        {
            isJump = false;
            //Debug.Log("jump is false");
        }
        //Debug.LogError(collision.gameObject.name);
        if (collision.gameObject.name == "magic") {
            isPlaying = false;
            OnChangeGameState(GameController.GameState.Victory);
            M_rigidbody.useGravity = false;
            M_rigidbody.transform.localPosition = new Vector3(1,8,86.8f);
            anim.Play("run_b");
        }
    }
    //开始游戏
    public void StartNewGame(Vector3 bornPos){
        isPlaying = true;
        curHealth--;
        gameObject.SetActive(false);
        transform.position = bornPos;
        transform.localEulerAngles = Vector3.zero;
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
            Debug.LogError("jump*****");
            isJump = true;
             _curPlayerState = PlayerState.Jump;
        }
        //Debug.Log("PlayerJump*******"+(Vector3.up*jumpSpeed));
    }

    public void IsStartMove(bool isStart){
        if (!isPlaying)
            return;
        isMoving = isStart;
        if(!isStart && _curPlayerState != PlayerState.Jump){
            // anim.stop
            // anim.GetCurrentAnimatorClipInfo(0).sto
            _curPlayerState = PlayerState.Standy;
             Debug.LogError("standby****");
            anim.Play("standby");
        }
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
            if(_curPlayerState != PlayerState.Jump){
                 _curPlayerState = PlayerState.Standy;
                Debug.LogError("standby****");

                anim.Play("standby");
            }

        }
    }

    private void UpdatePlayerState(){
        var ani = anim.GetCurrentAnimatorStateInfo(0);
        isJump = ani.IsName("jump");
        if(_curPlayerState == PlayerState.Move){
            // string aniName = anim.GetCurrentAnimatorClipInfo(0);
            if(!ani.IsName("jump") ){
                anim.Play("run_b");
            }
        }
        
    }

    public void InitPlayerInitData(){
        curHealth = 3;
    }

    public void ResetStartPos(Vector3 bornPos)
    {
        transform.position = bornPos;
        transform.localEulerAngles = Vector3.zero;
    }
    public IEnumerator PlayPlayerAni(Vector3 endPos) {
        isPlaying = false;
        Destroy(M_rigidbody);
        Vector3 detal = (endPos - transform.position)*Time.deltaTime;
        //Camera.main.transform.SetParent(transform);
        //Camera.main.transform.localPosition = new Vector3(0, 1.8f, -2.1f);
        //var _path = new Vector3[] {transform.position, endPos };
        //Tweener tweener = transform.DOPath(_path, 10f);
        //yield return new WaitForSeconds(10f);
        //tweener.Pause();
        while (Vector3.Distance(transform.position, endPos) > 0.1f) {
            //M_rigidbody.MovePosition(transform.position + endPos*0.001f);
            transform.position = Vector3.MoveTowards(transform.position, endPos, Time.deltaTime*20);//Vector3.Lerp(transform.position,endPos,Time.deltaTime*0.5f);
            yield return new WaitForEndOfFrame();
        }
        //transform.
        anim.Play("standby", 0, 0f);
    }

}
