using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    public class UIVerticalNavigationGroup : UINavigationGroupBase
    {
        protected override void OnMove(Vector2 axis)
        {
            if (axis.y > 0.5f && moveUp != null)
            {
                Deselect();

                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = 0;
                    if (moveUp != null)
                    {
                        navigationController.SetCurrentElement(moveUp);
                    }
                }
                else
                {
                    navigationController.SetCurrentElement(this);
                }
            }
            else if (axis.y < -0.5f && moveDown != null)
            {
                Deselect();
                currentIndex++;
                if (currentIndex >= ActiveChildrenCount)
                {
                    currentIndex = ActiveChildrenCount - 1;
                    if (moveDown != null)
                    {
                        navigationController.SetCurrentElement(moveDown);
                    }
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