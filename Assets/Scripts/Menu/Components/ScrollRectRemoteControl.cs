using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollRectRemoteControl : MonoBehaviour, IMoveHandler
{

    [SerializeField]
    private RectTransform contentRect;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float min;
    [SerializeField]
    private float max;

    public void OnMove(AxisEventData eventData)
    {
        Vector2 anchorPosition = contentRect.anchoredPosition;
        switch (eventData.moveDir)
        {
            case MoveDirection.Up:
                anchorPosition.y = Mathf.Clamp(anchorPosition.y + speed * Time.deltaTime, min, max);
                contentRect.anchoredPosition = anchorPosition;
                break;
            case MoveDirection.Down:
                anchorPosition.y = Mathf.Clamp(anchorPosition.y - speed * Time.deltaTime, min, max);
                contentRect.anchoredPosition = anchorPosition;
                break;
        }
    }

    public void SetMin(float newMin) => min = newMin;
}
