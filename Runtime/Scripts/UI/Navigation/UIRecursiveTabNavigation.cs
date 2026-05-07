using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.UI.Navigation
{
    public class UIRecursiveTabNavigation : UITabNavigation
    {
        protected override void OnMove(Vector2 axis)
        {
            if (!IsActiveNavigation) return;
            FindRecursively();
            base.OnMove(axis);
        }

        private void FindRecursively()
        {
            var elements = navigationController.GetRegisteredElements();
            var directions = new List<NavigationDirection>();
            foreach (var element in elements)
            {
                if (!directions.Contains(NavigationDirection.Right) &&
                    TryGetElement(NavigationDirection.Left, element, out var result))
                {
                    directions.Add(NavigationDirection.Right);
                    moveRight = result as UINavigationBase;
                }
                else if (!directions.Contains(NavigationDirection.Left) &&
                         TryGetElement(NavigationDirection.Right, element, out result))
                {
                    directions.Add(NavigationDirection.Left);
                    moveLeft = result as UINavigationBase;
                }
                else if (!directions.Contains(NavigationDirection.Down) &&
                         TryGetElement(NavigationDirection.Up, element, out result))
                {
                    directions.Add(NavigationDirection.Down);
                    moveDown = result as UINavigationBase;
                }
                else if (!directions.Contains(NavigationDirection.Up) &&
                         TryGetElement(NavigationDirection.Down, element, out result))
                {
                    directions.Add(NavigationDirection.Up);
                    moveUp = result as UINavigationBase;
                }

                if (directions.Count >= 4) break;
            }
        }

        private bool TryGetElement(NavigationDirection fromDirection,
            IUINavigationElement elementFrom, out IUINavigationElement result)
        {
            if (elementFrom.IsActiveNavigation && elementFrom.TryGetNavigationElement(fromDirection, out var from))
            {
                if (Equals(from))
                {
                    result = elementFrom;
                    return true;
                }
            }

            result = null;
            return false;
        }
    }
}