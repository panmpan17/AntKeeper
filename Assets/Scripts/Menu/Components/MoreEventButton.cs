using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace UnityEngine.UI
{
    [AddComponentMenu("UI/More Event Button")]
    public class MoreEventButton : Button
    {
        public PointerClickWithEventData onPointerDown = new PointerClickWithEventData();
        public PointerClickWithEventData onPointerUp = new PointerClickWithEventData();

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            onPointerDown.Invoke(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            onPointerUp.Invoke(eventData);
        }

        public class PointerClickWithEventData : UnityEvent<PointerEventData>
        {}
    }
}