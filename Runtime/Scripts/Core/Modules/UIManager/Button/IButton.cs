using System;
using UnityEngine.UI;

namespace DosinisSDK.Core
{
    public interface IButton : IInteractableElement
    {
        Graphic Image { get; }
        event Action OnClick;
        event Action OnPointerEntered;
        event Action OnPointerExited;
        event Action OnPressedIn;
        event Action OnReleased;
    }
}