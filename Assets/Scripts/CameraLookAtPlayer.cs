using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAtPlayer : MonoBehaviour
{
    public Transform target;    //用于编辑器中绑定玩家
    public float smoothing = 5f;    //用于计算顺滑度
    Vector3 offset;
    public Vector3 m_victoryCameraOff;
    void Start()
    {
        //首先初始化的时候保存相机和玩家的相对位置
        offset = transform.position - target.position;
        m_victoryCameraOff = offset;
    }
    bool isTurnRotate = false;
    //这里不要使用FixedUpdate, 移动端屏幕会有视觉卡顿
    void Update()
    {
        if (GameController.Instance.m_GameState == GameController.GameState.Play )
        {
            //计算出相机跟随的位置
            //Vector3 targetCamPos = new Vector3(target.position.x+offset.x,transform.position.y,target.position.z+offset.z);//target.position + offset;
            Vector3 targetCamPos = target.position + offset;
            //设置相机的位置,这里用到了Vector3.Lerp,是一个差值计算,使得移动更柔和.但是会略微消耗计算量
            //由于主摄像机只有1个,所以可以忽略这个计算量的消耗
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        }else if (GameController.Instance.m_GameState == GameController.GameState.Victory)
        {
            //if (!isTurnRotate) {
            //    transform.localEulerAngles += new Vector3(0,45,0);
            //    isTurnRotate = true;
            //}
            //计算出相机跟随的位置
            //Vector3 targetCamPos = new Vector3(target.position.x+offset.x,transform.position.y,target.position.z+offset.z);//target.position + offset;
            Vector3 targetCamPos = target.position + m_victoryCameraOff;
            //设置相机的位置,这里用到了Vector3.Lerp,是一个差值计算,使得移动更柔和.但是会略微消耗计算量
            //由于主摄像机只有1个,所以可以忽略这个计算量的消耗
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime*2);
        }
    }
   
}