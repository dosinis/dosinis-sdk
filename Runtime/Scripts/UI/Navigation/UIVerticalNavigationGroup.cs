using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    public class UIVerticalNavigationGroup : UINavigationGroupBase
    {
        protected override void OnMove(Vector2 axis)
        {
            if (axis.y > 0.5f && moveUp != null)
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = 0;
                    navigationController.SetCurrentElement(moveUp);
                }
                else
                {
                    navigationController.SetCurrentElement(this);
                }

                navigationController.SetCurrentElement(moveUp);
            }
            else if (axis.y < -0.5f && moveDown != null)
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
            else if (axis.x < -0.5f && moveLeft != null)
            {
                navigationController.SetCurrentElement(moveLeft);
            }
            else if (axis.x > 0.5f && moveRight != null)
            {
                navigationController.SetCurrentElement(moveRight);
            }
        }
    }
}