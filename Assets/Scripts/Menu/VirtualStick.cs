using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualStick : MonoBehaviour, IDragHandler, IEndDragHandler
{
    // private ve
    [SerializeField]
    private Transform canvasTransform;
    [SerializeField]
    private RectTransform handle;
    [SerializeField]
    private float radius;

    private Vector2 _originPosition;

    public event System.Action<Vector2> OnMovementAxisChange;

    void Awake()
    {
        _originPosition = handle.anchoredPosition;
    }


    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - (Vector2)transform.position;
        delta = Vector2.ClampMagnitude(delta, radius * canvasTransform.localScale.x);
        handle.position = transform.position + (Vector3)delta;
        OnMovementAxisChange?.Invoke(delta.normalized);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        handle.anchoredPosition = _originPosition;
        OnMovementAxisChange?.Invoke(Vector2.zero);
    }
}
