using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using MPack;
using TMPro;


namespace UnityEngine.UI
{
    public class ToggleSwitch : Selectable, IPointerClickHandler, ISubmitHandler
    {
        [SerializeField]
        private bool isOn;

        [SerializeField]
        private Image handleImage;
        private RectTransform handle;
        [SerializeField]
        private TextMeshProUGUI text;

        [SerializeField]
        private EventReference triggerEvent;

        [SerializeField]
        private SwitchStateVisualization onState;
        [SerializeField]
        private SwitchStateVisualization offState;

        public bool IsOn => isOn;

        protected override void Awake()
        {
            base.Awake();
            handle = handleImage.GetComponent<RectTransform>();
            ApplyState(isOn ? onState : offState);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsInteractable())
                ToggleChoice();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (IsInteractable())
                ToggleChoice();
        }

        public void ChangeState(bool isOn)
        {
            this.isOn = isOn;
            ApplyState(isOn ? onState : offState);
            triggerEvent?.Invoke(isOn);
        }
        public void ChangeStateWithoutTriggerEvent(bool isOn)
        {
            this.isOn = isOn;
            ApplyState(isOn ? onState : offState);
        }

        public void ToggleChoice()
        {
            isOn = !isOn;
            ApplyState(isOn ? onState : offState);
            triggerEvent?.Invoke(isOn);
        }

        void ApplyState(SwitchStateVisualization visualization)
        {
            handle.anchoredPosition = visualization.HandleAnchorPosition;
            handleImage.color = visualization.HandleColor.Value;

            if (text != null)
                text.color = visualization.TextColor.Value;
        }

        [System.Serializable]
        public struct SwitchStateVisualization
        {
            public Vector2 HandleAnchorPosition;
            public ColorReference HandleColor;
            public ColorReference TextColor;
            // public UnityEvent Event;
        }
    }
}