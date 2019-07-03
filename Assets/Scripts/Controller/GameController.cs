using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
    [SerializeField] private GameObject m_prefab;
    [SerializeField] private Transform m_EndPos;
    [SerializeField] private GameObject m_GameContentObj;
    [SerializeField] private GameObject m_VictoryObj1;
    [SerializeField] private GameObject m_GameUIObj;

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
        //UnityEngine.QualityLevel.
        Debug.LogError("QualitySettings.GetQualityLevel***********: "+ QualitySettings.GetQualityLevel());
        QualitySettings.SetQualityLevel(4, true);
        Debug.LogError("QualitySettings.GetQualityLevel￥￥￥￥￥￥￥￥: " + QualitySettings.GetQualityLevel());

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
        }else if(state == GameState.Victory) {
            Debug.LogError("victory**************8");
            StartCoroutine(PlayVictory());
        }
        Debug.LogError("prePositonZ: " +prePositonZ);
    }


    public void StartPlay(){
        startObj.SetActive(false);
        m_GameUIObj.SetActive(true);
        m_GameContentObj.SetActive(true);
        m_VictoryObj1.SetActive(false);
        m_GameState = GameState.Play;
        player.StartNewGame(GetBornAtPos());
        StartCoroutine(Timing());
    }

    private Vector3 GetBornAtPos(){
        var _count = m_BornAtParent.childCount;
        // bool isFind = false;
        //return m_BornAtParent.GetChild(5).position;
        for (int i = _count-1 ;i >= 0 ;i--){
            Debug.LogError("m_BornAtParent.GetChild(i).position.z: "+m_BornAtParent.GetChild(i).position.z);
            if(prePositonZ >= m_BornAtParent.GetChild(i).position.z){
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

    #region victory
    IEnumerator PlayVictory() {
        //player.transform.DOLocalMoveY(1f,2);
        //yield return new WaitForSeconds(2f);
        m_GameUIObj.SetActive(false);
        m_VictoryObj1.SetActive(true);
        var _victory = (GameObject)Instantiate(m_prefab);
        _victory.SetActive(false);
        _victory.transform.SetParent(GameObject.Find("Canvas").transform);
        _victory.transform.localPosition = Vector3.zero;
        _victory.transform.localScale = Vector3.zero;
        _victory.SetActive(true);
        var _canvasGroup = _victory.GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
        _canvasGroup.DOFade(1, 0.5f).SetEase(Ease.OutQuad);
        _victory.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuad);
        //Debug.LogError("m_BornAtParent.GetChild(0).position: " + m_BornAtParent.GetChild(0).position);
        yield return new WaitForSeconds(0.5f);
        player.ResetStartPos(m_BornAtParent.GetChild(0).position);
        yield return new WaitForSeconds(5f);

        _canvasGroup.DOFade(0, 0.2f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.2f);
        Destroy(_victory);
        //m_GameContentObj.SetActive(false);
        yield return new WaitForSeconds(1f);
       
        yield return player.PlayPlayerAni(m_EndPos.position);
        m_GameContentObj.SetActive(false);
    }
    #endregion
}

