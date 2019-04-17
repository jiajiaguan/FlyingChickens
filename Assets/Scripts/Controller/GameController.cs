using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    //public delegate void OnPlayerMove(float horizonta,float ververtical);
public class GameController : MonoBehaviour
{
    public ScrollCircle m_scrollCircle;
    [SerializeField]
    private GameObject startObj;
    [SerializeField]
    private Transform m_BornAt;
    public PlayerMove player;
    public enum GameState{
        None,
        Init,
        Start,
        Play,
        GameOver,
        Victory,
        Fall
    }
    public GameState m_GameState = GameState.None;
    private static GameController _instance;
    public static GameController Instance{
        get{
            //if(_instance)
            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
        m_GameState = GameState.Init;
        GameData.Instance.InitData();//数据初始化
    }
    // Use this for initialization
    void Start()
    {
        m_scrollCircle.OnPlayerMoveAction = player.playerMove;
        m_scrollCircle.OnIsStartMoveAction = player.IsStartMove;
        player.OnChangeGameState = OnChangeGameState;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnChangeGameState(GameState state)
    {
           m_GameState = state;
        if(state == GameState.GameOver){
            startObj.SetActive(true);
            m_GameState = GameState.Start;
        }
    }


    public void StartPlay(){
        startObj.SetActive(false);
        m_GameState = GameState.Play;
        player.StartNewGame(m_BornAt.transform.position);
    }
}

