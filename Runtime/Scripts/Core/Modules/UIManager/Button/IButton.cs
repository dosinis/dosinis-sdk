using System;
using UnityEngine.UI;

namespace DosinisSDK.Core
{
    public interface IButton
    {
        Graphic Image { get; }
        bool Interactable { get; set; }
        
        event Action OnClick;
        event Action OnPointerEntered;
        event Action OnPointerExited;
        event Action OnPressedIn;
        event Action OnReleased;
    }
}
