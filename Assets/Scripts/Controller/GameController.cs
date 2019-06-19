using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    //public delegate void OnPlayerMove(float horizonta,float ververtical);
public class GameController : MonoBehaviour
{
    public ScrollCircle m_scrollCircle;
    [SerializeField]
    private GameObject startObj;
    [SerializeField]
    private Transform m_BornAt;
    [SerializeField]private Transform m_BornAtParent;
    [SerializeField] private Text m_TimeText;
    public PlayerMove player;
    private float prePositonZ;
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
        prePositonZ = m_BornAtParent.GetChild(0).position.z;
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
            InitGameData();
            player.InitPlayerInitData();
            startObj.SetActive(true);
            m_GameState = GameState.Start;
        }else if(state == GameState.Fall){
            prePositonZ = player.transform.position.z;
            m_GameState = GameState.Play;
            player.StartNewGame(GetBornAtPos());
        }
        Debug.LogError("prePositonZ: " +prePositonZ);
    }


    public void StartPlay(){
        startObj.SetActive(false);
        m_GameState = GameState.Play;
        player.StartNewGame(GetBornAtPos());
        StartCoroutine(Timing());
    }

    private Vector3 GetBornAtPos(){
        var _count = m_BornAtParent.childCount;
        // bool isFind = false;
        //return m_BornAtParent.GetChild(4).position;
        for (int i = _count-1 ;i >= 0 ;i--){
            Debug.LogError("m_BornAtParent.GetChild(i).position.z: "+m_BornAtParent.GetChild(i).position.z);
            if(prePositonZ >= m_BornAtParent.GetChild(i).position.z){
                Debug.LogError("*************"+i);
                return m_BornAtParent.GetChild(i).position;
            }
        }
        return m_BornAtParent.GetChild(_count-1).position;
    }

    private void InitGameData(){
        prePositonZ = m_BornAtParent.GetChild(0).position.z;
        m_GameState = GameState.Init;
        GameData.Instance.InitData();//数据初始化
        time = 0;
        m_TimeText.text = string.Format("时间：{0}", time);
    }
    private float time = 0;
    IEnumerator Timing() {
        while(m_GameState == GameState.Play) {
            yield return new WaitForSeconds(1);
            Timing();
            time++;
            m_TimeText.text = string.Format("时间：{0}",time);

        }      
    }
}

