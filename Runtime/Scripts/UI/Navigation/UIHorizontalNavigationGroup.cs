using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    public class UIHorizontalNavigationGroup : UINavigationGroupBase
    {
        protected override void OnMove(Vector2 axis)
        {
            if (axis.y > 0.5f && moveUp != null)
            {
                navigationController.SetCurrentElement(moveUp);
            }
            else if (axis.y < -0.5f && moveDown != null)
            {
                navigationController.SetCurrentElement(moveDown);
            }
            else if (axis.x < -0.5f && moveLeft != null)
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = 0;
                    navigationController.SetCurrentElement(moveLeft);
                }
                else
                {
                    navigationController.SetCurrentElement(this);
                }
            }
            else if (axis.x > 0.5f && moveRight != null)
            {
                currentIndex++;
                if (currentIndex >= children.Count)
                {
                    currentIndex = children.Count - 1;
                    navigationController.SetCurrentElement(moveRight);
                }
                else
                {
                    navigationController.SetCurrentElement(this);
                }
            }
        }
    }
}