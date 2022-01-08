using System;

namespace DosinisSDK.UI
{
    public interface IButtonAnimation
    {
        void Init();

        void PressAnimation(Action callback);

        void ReleaseAnimation(Action callback);
    }
}