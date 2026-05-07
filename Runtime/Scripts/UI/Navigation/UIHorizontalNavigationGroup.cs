using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    public class UIHorizontalNavigationGroup : UINavigationGroupBase
    {
        protected override void OnMove(Vector2 axis)
        {
            if (axis.y > 0.5f && moveUp is { IsActiveNavigation: true })
            {
                Deselect();
                navigationController.SetCurrentElement(moveUp);
            }
            else if (axis.y < -0.5f && moveDown is { IsActiveNavigation: true })
            {
                Deselect();
                navigationController.SetCurrentElement(moveDown);
            }
            else if (axis.x < -0.5f)
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = 0;
                    if (moveLeft is { IsActiveNavigation: true })
                    {
                        Deselect();
                        navigationController.SetCurrentElement(moveLeft);
                    }
                }
                else
                {
                    Deselect();
                    navigationController.SetCurrentElement(this);
                }
            }
            else if (axis.x > 0.5f)
            {
                currentIndex++;
                if (currentIndex >= ActiveChildrenCount)
                {
                    currentIndex = ActiveChildrenCount - 1;
                    if (moveRight is { IsActiveNavigation: true })
                    {
                        Deselect();
                        navigationController.SetCurrentElement(moveRight);
                    }
                }
                else
                {
                    Deselect();
                    navigationController.SetCurrentElement(this);
                }
            }
        }
    }
}