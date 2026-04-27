using DosinisSDK.Core;
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
            else if (axis.x < -0.5f)
            {
                Deselect();
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = 0;
                    if (moveLeft != null)
                    {
                        navigationController.SetCurrentElement(moveLeft);
                    }
                }
                else
                {
                    navigationController.SetCurrentElement(this);
                }
            }
            else if (axis.x > 0.5f)
            {
                Deselect();
                currentIndex++;
                if (currentIndex >= ActiveChildrenCount)
                {
                    currentIndex = ActiveChildrenCount - 1;
                    if (moveRight != null)
                    {
                        navigationController.SetCurrentElement(moveRight);
                    }
                }
                else
                {
                    navigationController.SetCurrentElement(this);
                }
            }
        }
    }
}