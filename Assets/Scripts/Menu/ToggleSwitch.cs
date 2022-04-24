using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using MPack;
using TMPro;


namespace UnityEngine.UI
{
    public class ToggleSwitch : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private bool isOn;

        [SerializeField]
        private Image handleImage;
        private RectTransform handle;
        [SerializeField]
        private TextMeshProUGUI text;

        [SerializeField]
        private SwitchStateVisualization onState;
        [SerializeField]
        private SwitchStateVisualization offState;

        public bool IsOn => isOn;

        void Awake()
        {
            handle = handleImage.GetComponent<RectTransform>();
            ApplyState(isOn ? onState : offState);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ToggleChoice();
        }

        public void ChangeState(bool isOn)
        {
            this.isOn = isOn;

            if (isOn)
            {
                ApplyState(onState);
                onState.Event.Invoke();
            }
            else
            {
                ApplyState(offState);
                offState.Event.Invoke();
            }
        }
        public void ChangeStateWithoutTriggerEvent(bool isOn)
        {
            this.isOn = isOn;
            ApplyState(isOn ? onState : offState);
        }

        public void ToggleChoice()
        {
            isOn = !isOn;

            if (isOn)
            {
                ApplyState(onState);
                onState.Event.Invoke();
            }
            else
            {
                ApplyState(offState);
                offState.Event.Invoke();
            }
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
            public UnityEvent Event;
        }
    }
}