using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace DosinisSDK.Core
{
    public class ClickThroughButton : Button
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (Interactable == false)
                return;
            
            base.OnPointerClick(eventData);
            
            var currentRaycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, currentRaycastResults);
            
            foreach (var result in currentRaycastResults)
            {
                if (result.gameObject == gameObject)
                    continue;
                
                var underlyingButton = result.gameObject.GetComponent<Button>();

                if (underlyingButton == null || !underlyingButton.Interactable)
                    continue;
                
                var newEventData = new PointerEventData(EventSystem.current)
                {
                    pointerId = eventData.pointerId,
                    position = eventData.position,
                    delta = eventData.delta,
                    pressPosition = eventData.pressPosition,
                    pointerCurrentRaycast = result,
                };
                
                underlyingButton.OnPointerClick(newEventData);
                break;
            }
        }
    }
}
