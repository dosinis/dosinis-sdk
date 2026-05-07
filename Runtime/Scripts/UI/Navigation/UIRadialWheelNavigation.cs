using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    public class UIRadialWheelNavigation : UINavigationBase
    {
        [SerializeField] private List<GameObject> elements = new();
        private int currentIndex = 0;

        public override GameObject Target => elements[currentIndex];

        protected override void OnMove(Vector2 axis)
        {
            if (axis == Vector2.zero) return;

            var nextIndex = GetIndex(axis);
            if (nextIndex != currentIndex)
            {
                Deselect();
                currentIndex = nextIndex;
                navigationController.SetCurrentElement(this);
            }
        }

        private int GetIndex(Vector2 axis)
        {
            float angle = Mathf.Atan2(axis.x, axis.y) * Mathf.Rad2Deg;
            if (angle < 0)
                angle += 360f;

            float segmentSize = 360f / elements.Count;
            float shifted = (angle + segmentSize * 0.5f) % 360f;

            return Mathf.FloorToInt(shifted / segmentSize);
        }
    }
}