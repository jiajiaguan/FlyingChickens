using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ScrollCircle : ScrollRect
{
    // 半径
    private float _mRadius = 0f;

    // 距离
    private const float Dis = 0.5f;
    public System.Action<float,float> OnPlayerMoveAction;
    public System.Action<bool> OnIsStartMoveAction;
    protected override void Start()
    {
        base.Start();

        // 能移动的半径 = 摇杆的宽 * Dis
        _mRadius = content.sizeDelta.x * Dis;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        // 获取摇杆，根据锚点的位置。
        var contentPosition = content.anchoredPosition;

        // 判断摇杆的位置 是否大于 半径
        if (contentPosition.magnitude > _mRadius)
        {
            // 设置摇杆最远的位置
            contentPosition = contentPosition.normalized * _mRadius;
            SetContentAnchoredPosition(contentPosition);
        }

        // 最后 v2.x/y 就跟 Input中的 Horizontal Vertical 获取的值一样 
        var v2 = content.anchoredPosition.normalized;
        if(OnPlayerMoveAction != null){
            OnPlayerMoveAction(v2.x,v2.y);
        }
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if(OnIsStartMoveAction != null){
            OnIsStartMoveAction(true);
        }
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if(OnIsStartMoveAction != null){
            OnIsStartMoveAction(false);
        }
    }
}