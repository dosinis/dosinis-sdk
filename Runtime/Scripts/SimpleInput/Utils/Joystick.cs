using UnityEngine;
using UnityEngine.EventSystems;

namespace DosinisSDK.SimpleInput.Utils
{
    public class Joystick : InputHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private float movementRange = 50;
        [SerializeField] private bool fixedPosition = false;

        // Private

        private Vector2 startPos;
        private Vector2 pointerDownPos;

        private RectTransform parentRect;
        private RectTransform rect;

        private void Awake()
        {
            rect = (RectTransform)transform;
            parentRect = transform.parent.GetComponentInParent<RectTransform>();

            startPos = rect.anchoredPosition;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, eventData.pressEventCamera, out pointerDownPos);
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, eventData.pressEventCamera, out var position);

            var delta = position - pointerDownPos;

            var clampedDelta = Vector2.ClampMagnitude(delta, movementRange);

            rect.anchoredPosition = startPos + (fixedPosition ? clampedDelta : delta);

            Direction = new Vector2(clampedDelta.x / movementRange, clampedDelta.y / movementRange);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            rect.anchoredPosition = startPos;
            Direction = Vector2.zero;
        }
    }
}
