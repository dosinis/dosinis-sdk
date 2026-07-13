using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    public class UIVerticalNavigationGroup : UINavigationGroupBase
    {
        protected override void OnMove(Vector2 axis)
        {
            int newIndex = currentIndex;
            if (axis.y > 0.5f )
            {
                newIndex--;
                if (newIndex < 0)
                {
                    newIndex = 0;
                    if (moveUp is { IsActiveNavigation: true })
                    {
                        Deselect();
                        currentIndex = newIndex;
                        navigationController.SetCurrentElement(moveUp);
                    }
                }
                else
                {
                    Deselect();
                    currentIndex = newIndex;
                    navigationController.SetCurrentElement(this);
                }
            }
            else if (axis.y < -0.5f )
            {
                newIndex++;
                if (newIndex >= ActiveChildrenCount)
                {
                    newIndex = ActiveChildrenCount - 1;
                    if (moveDown is { IsActiveNavigation: true })
                    {
                        Deselect();
                        currentIndex = newIndex;
                        navigationController.SetCurrentElement(moveDown);
                    }
                }
                else
                {
                    Deselect();
                    currentIndex = newIndex;
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