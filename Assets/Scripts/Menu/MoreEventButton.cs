using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace UnityEngine.UI
{
    [AddComponentMenu("UI/More Event Button")]
    public class MoreEventButton : Button
    {
        public ButtonClickedEvent onPointerDown;
        public ButtonClickedEvent onPointerUp;

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            onPointerDown.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            onPointerUp.Invoke();
        }
    }
}