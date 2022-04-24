using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private bool baseFollowByHandle;
    [SerializeField]
    private bool moveBaseOnFirstTouch;
    [SerializeField]
    private Rect moveConstrainRect;

    [SerializeField]
    private Transform wheelBase;
    [SerializeField]
    private RectTransform handle;
    private Transform canvasTransform;
    [SerializeField]
    private float radius;

    private Vector2 _originPosition;
    private RectTransform _rectTransform;

    public event System.Action<Vector2> OnMovementAxisChange;

    void Awake()
    {
        _originPosition = handle.anchoredPosition;

        canvasTransform = transform.root;
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ChangeBasePosition(eventData.position);
        handle.anchoredPosition = _originPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - (Vector2)wheelBase.position;
        float scaledRadius = radius * canvasTransform.localScale.x;

        if (delta.sqrMagnitude >= scaledRadius * scaledRadius)
        {
            delta = Vector2.ClampMagnitude(delta, scaledRadius);

            if (baseFollowByHandle)
            {
                handle.position = eventData.position;
                ChangeBasePosition(eventData.position - delta);
            }
            else
            {
                handle.position = wheelBase.position + (Vector3)delta;
            }
        }
        else
        {
            handle.position = eventData.position;
        }

        OnMovementAxisChange?.Invoke(delta.normalized);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        handle.anchoredPosition = _originPosition;
        OnMovementAxisChange?.Invoke(Vector2.zero);
    }

    void ChangeBasePosition(Vector3 position)
    {
        Rect rect = moveConstrainRect;
        rect.center *= transform.root.localScale.x;
        rect.size *= transform.root.localScale.x;
        position.x = Mathf.Clamp(position.x, rect.xMin, rect.xMax);
        position.y = Mathf.Clamp(position.y, rect.yMin, rect.yMax);

        wheelBase.position = position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawCube(moveConstrainRect.center * transform.root.localScale.x, moveConstrainRect.size * transform.root.localScale.x);
    }
}
