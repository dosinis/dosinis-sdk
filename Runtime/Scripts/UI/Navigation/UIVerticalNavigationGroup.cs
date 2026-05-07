using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    public class UIVerticalNavigationGroup : UINavigationGroupBase
    {
        protected override void OnMove(Vector2 axis)
        {
            if (axis.y > 0.5f && moveUp is { IsActiveNavigation: true })
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = 0;
                    if (moveUp is { IsActiveNavigation: true })
                    {
                        Deselect();
                        navigationController.SetCurrentElement(moveUp);
                    }
                }
                else
                {
                    Deselect();
                    navigationController.SetCurrentElement(this);
                }
            }
            else if (axis.y < -0.5f && moveDown is { IsActiveNavigation: true })
            {
                currentIndex++;
                if (currentIndex >= ActiveChildrenCount)
                {
                    currentIndex = ActiveChildrenCount - 1;
                    if (moveDown is { IsActiveNavigation: true })
                    {
                        Deselect();
                        navigationController.SetCurrentElement(moveDown);
                    }
                }
                else
                {
                    Deselect();
                    navigationController.SetCurrentElement(this);
                }
            }
            else if (axis.x < -0.5f && moveLeft is { IsActiveNavigation: true })
            {
                Deselect();
                navigationController.SetCurrentElement(moveLeft);
            }
            else if (axis.x > 0.5f && moveRight is { IsActiveNavigation: true })
            {
                Deselect();
                navigationController.SetCurrentElement(moveRight);
            }
        }
    }
}