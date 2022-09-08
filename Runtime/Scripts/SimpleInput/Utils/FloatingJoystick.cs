using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DosinisSDK.SimpleInput.Utils
{
    public class FloatingJoystick : InputHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private float movementRange = 50;
        [SerializeField] private bool fixedPosition = false;
        [SerializeField] private RectTransform stick;
        [SerializeField] private RectTransform arrow;
        [SerializeField] private bool alwaysVisible = true;

        // Private

        private Vector2 startPos;
        private Vector2 pointerDownPos;
        private Vector2 initialHolderPos;

        private RectTransform inputRect;
        private RectTransform stickHolder;

        private Image arrowImage;

        private void Awake()
        {
            inputRect = transform.GetComponent<RectTransform>();
            
            if (arrow)
                arrowImage = arrow.GetComponent<Image>();
            
            stickHolder = (RectTransform)stick.parent;
            initialHolderPos = stickHolder.anchoredPosition;
            startPos = stick.anchoredPosition;

            if (alwaysVisible == false)
                stickHolder.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            OnPointerUp(null);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (InputSystem.Enabled == false)
                return;

            stickHolder.gameObject.SetActive(true);
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(inputRect, eventData.position,
                eventData.pressEventCamera, out pointerDownPos);
            
            stickHolder.anchoredPosition = pointerDownPos;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (InputSystem.Enabled == false)
                return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(inputRect, eventData.position,
                eventData.pressEventCamera, out var position);

            var delta = position - pointerDownPos;

            var clampedDelta = Vector2.ClampMagnitude(delta, movementRange);

            stick.anchoredPosition = startPos + (fixedPosition ? clampedDelta : delta);

            Direction = new Vector2(clampedDelta.x / movementRange, clampedDelta.y / movementRange);

            float angle = Mathf.Atan2(Direction.x, Direction.y) * Mathf.Rad2Deg;

            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, -angle));
            
            if (arrow)
            {
                arrow.rotation = rotation;
                arrowImage.color = new Color(1, 1, 1, Mathf.Clamp(Mathf.Abs(Direction.x) + Mathf.Abs(Direction.y), 0, 1));
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (InputSystem.Enabled == false)
                return;

            if (alwaysVisible == false)
            {
                stickHolder.gameObject.SetActive(false);
            }
            else
            {
                stickHolder.anchoredPosition = initialHolderPos;
            }
            
            stick.anchoredPosition = startPos;
            Direction = Vector2.zero;
        }
    }
}