using System;

namespace DosinisSDK.UI
{
    public interface IButtonAnimation
    {
        void Init();

        void PressAnimation();

        void ReleaseAnimation();
    }
}