using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script requires you to have setup your animator with 3 parameters, "InputMagnitude", "InputX", "InputZ"
//With a blend tree to control the inputmagnitude and allow blending between animations.
[RequireComponent(typeof(CharacterController))]
public class MovementInput : MonoBehaviour
{

    public float InputX;
    public float InputZ;
    public Vector3 desiredMoveDirection;
    public bool blockRotationPlayer;
    public float desiredRotationSpeed;
    //public Animator anim;
    public float Speed;
    public float moveSpeed;
    public float allowPlayerRotation;
    public Camera cam;
    public CharacterController controller;
    public bool isGrounded;
    private float verticalVel;
    private Vector3 moveVector;

    // Use this for initialization
    void Start()
    {
        //anim = this.GetComponent<Animator>();
        cam = Camera.main;
        controller = this.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        InputMagnitude();
        //解决人物不受重力的问题
        isGrounded = controller.isGrounded;
        if (isGrounded)
        {
            verticalVel -= 0;
        }
        else
        {
            verticalVel -= 2;
        }
        moveVector = new Vector3(0, verticalVel, 0);
        controller.Move(moveVector);

    }

    private void PlayerMoveAndRotation()
    {
        float InputX = 3;// anim.GetFloat("InputX");
            float InputZ = 3;//anim.GetFloat("InputZ");
        Debug.Log("cam.transform.right: " + cam.transform.right);
        Vector3 desiredForward = cam.transform.right;//获取camera的前轴向，和右轴向，让character旋转完后正（背）对摄像机，根据用户输入值大小判断负值则转到反方向
        Vector3 desiredRight = cam.transform.forward;
        desiredMoveDirection = desiredForward * InputX + desiredRight * InputZ;
        if (blockRotationPlayer == false)
        { 
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
        }

        controller.SimpleMove(desiredMoveDirection * moveSpeed);
    }

    private void InputMagnitude()
    {
        float InputX = Input.GetAxis("Horizontal");
        float InputZ = Input.GetAxis("Vertical");
        Debug.Log("InputX: "+ InputX);
        //anim.SetFloat("InputX", InputX);
        //anim.SetFloat("InputZ", InputZ);
        float inputMagnitude = (new Vector3(InputX*3, 0, InputZ*3)).sqrMagnitude;
        //anim.SetFloat("InputMagnitude", inputMagnitude);
        if (inputMagnitude > allowPlayerRotation) //在这里判断输入值不为0，才改变character的旋转值，防止character旋转方向变成(0,0,0)
        {
            //PlayerMoveAndRotation();
            controller.Move(new Vector3(InputX * 3, 0, InputZ * 3)* Time.deltaTime);
        }
    }
}

