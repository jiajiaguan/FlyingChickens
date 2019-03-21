using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    //public delegate void OnPlayerMove(float horizonta,float ververtical);
public class GameController : MonoBehaviour
{
    public ScrollCircle m_scrollCircle;

    public PlayerMove player;

    // Use this for initialization
    void Start()
    {
        m_scrollCircle.OnPlayerMoveAction = player.playerMove;
        m_scrollCircle.OnIsStartMoveAction = player.IsStartMove;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void PlayerJump(){

    }
}

