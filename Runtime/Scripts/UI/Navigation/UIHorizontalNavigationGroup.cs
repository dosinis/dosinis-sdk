using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    public class UIHorizontalNavigationGroup : UINavigationGroupBase
    {
        protected override void OnMove(Vector2 axis)
        {
            int newIndex = currentIndex;
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
                newIndex--;
                if (newIndex < 0)
                {
                    newIndex = 0;
                    if (moveLeft is { IsActiveNavigation: true })
                    {
                        Deselect();
                        currentIndex = newIndex;
                        navigationController.SetCurrentElement(moveLeft);
                    }
                }
                else
                {
                    Deselect();
                    currentIndex = newIndex;
                    navigationController.SetCurrentElement(this);
                }
            }
            else if (axis.x > 0.5f)
            {
                newIndex++;
                if (newIndex >= ActiveChildrenCount)
                {
                    newIndex = ActiveChildrenCount - 1;
                    if (moveRight is { IsActiveNavigation: true })
                    {
                        Deselect();
                        currentIndex = newIndex;
                        navigationController.SetCurrentElement(moveRight);
                    }
                }
                else
                {
                    Deselect();
                    currentIndex = newIndex;
                    navigationController.SetCurrentElement(this);
                }
            }
        }
    }
}