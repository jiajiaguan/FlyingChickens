using UnityEngine;
using System.Collections;

public class CamFollow : MonoBehaviour
{
    public GameObject player;
    public float updistan;  //设置距离Player上方偏移量
    public float fordistan; //设置距离Player前方偏移量
    Vector3 targetpso;  //摄像机目标位置
    public float speed = 1f;
    // 初始化找到Player
    void Start()
    {
        if (player == null)
        {
            player = GameObject.Find("Player");
        }

    }

    // 引用Letmego方法
    void LateUpdate()
    {
        Letmego();

    }
    //实现摄像机跟随
    void Letmego()
    {
        targetpso = new Vector3(0, updistan, player.transform.position.z+fordistan);//相对于Player的目标位置
        transform.localPosition = Vector3.Lerp(transform.position, targetpso, speed);//平滑移动到目标位置
        //transform.LookAt(player.transform);//摄像机Z轴指向Player
    }
}