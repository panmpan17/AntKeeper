using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private bool baseFollowByHandle;
    [SerializeField]
    private bool moveBaseOnFirstTouch;

    [SerializeField]
    private Transform wheelBase;
    [SerializeField]
    private RectTransform handle;
    [SerializeField]
    private float radius;

    private Vector2 _originPosition;
    private RectTransform _rectTransform;
    private Transform _canvasTransform;
    private CanvasGroup _wheelBaseCanvasGroup;

    public event System.Action<Vector2> OnMovementAxisChange;

    void Awake()
    {
        _originPosition = handle.anchoredPosition;

        _canvasTransform = transform.root;
        _rectTransform = GetComponent<RectTransform>();
        _wheelBaseCanvasGroup = wheelBase.GetComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _wheelBaseCanvasGroup.alpha = 1;
        ChangeBasePosition(eventData.position);
        handle.anchoredPosition = _originPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - (Vector2)wheelBase.position;
        float scaledRadius = radius * _canvasTransform.localScale.x;

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
        _wheelBaseCanvasGroup.alpha = 0.5f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _wheelBaseCanvasGroup.alpha = 0.5f;
    }

    void ChangeBasePosition(Vector3 position)
    {
        Rect rect = _rectTransform.rect;
        rect.center += (Vector2)transform.position;
        rect.size *= transform.root.localScale.x;
        position.x = Mathf.Clamp(position.x, rect.xMin, rect.xMax);
        position.y = Mathf.Clamp(position.y, rect.yMin, rect.yMax);

        wheelBase.position = position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);

        Rect rect = GetComponent<RectTransform>().rect;
        Gizmos.DrawCube((Vector3)rect.center + transform.position, rect.size * transform.root.localScale.x);
        Gizmos.DrawSphere(wheelBase.position, 10f);
    }
}
