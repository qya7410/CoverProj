using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour,IDragHandler,IPointerDownHandler,IPointerUpHandler
{
    public Vector2 GetiputDirection { set; get; }
    private Image bgImg;
    private Image joystickImg;

    private void Start()
    {
        bgImg = GetComponent<Image>();
        joystickImg = transform.GetChild(0).GetComponent<Image>();
    }


    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle
            (bgImg.rectTransform,eventData.position,eventData.pressEventCamera,out pos))
        {
            //先算出可运动范围，再给轴心点做一个偏移，
            Vector2 anchoredOffset = bgImg.rectTransform.sizeDelta / 2;
            GetiputDirection = Vector2.ClampMagnitude(pos - anchoredOffset, bgImg.rectTransform.sizeDelta.x / 2);
            joystickImg.rectTransform.anchoredPosition = GetiputDirection;

        }
        Debug.Log(GetiputDirection);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
        Debug.Log("按下");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GetiputDirection = Vector2.zero;
        joystickImg.rectTransform.anchoredPosition = Vector2.zero;
        Debug.Log("松手");
    }

}
