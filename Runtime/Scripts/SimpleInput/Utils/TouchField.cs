using UnityEngine;
using UnityEngine.EventSystems;

namespace DosinisSDK.SimpleInput.Utils
{
    public class TouchField : InputHandler, IBeginDragHandler, IDragHandler
    {
        [SerializeField] private float maxDragMagnitude = 50;
        [SerializeField] private float dragSmooth = 10;

        private Vector2 previousPos;
        private Vector2 currentPos;
        private RectTransform rect;

        private void Awake()
        {
            rect = (RectTransform)transform;
        }

        private void Update()
        {
            var delta = currentPos - previousPos;

            delta = Vector2.ClampMagnitude(delta, maxDragMagnitude);

            var normalizedDelta = new Vector2(delta.x / maxDragMagnitude, delta.y / maxDragMagnitude);

            Direction = Vector2.Lerp(Direction, normalizedDelta, dragSmooth * Time.deltaTime);

            previousPos = currentPos;
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out currentPos);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out previousPos);
        }
    }
}