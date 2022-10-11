using System;

namespace DosinisSDK.Core
{
    public interface IButtonAnimation
    {
        void Init();

        void PressAnimation();

        void ReleaseAnimation();

        void OnInteractableStateChanged(bool value);
    }
}