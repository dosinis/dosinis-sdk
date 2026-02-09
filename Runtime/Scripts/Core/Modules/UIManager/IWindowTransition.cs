using System;

namespace DosinisSDK.Core
{
    public interface IWindowTransition
    {
        void Init();
        void ShowTransition(Action done);
        void HideTransition(Action done);
        void ResetTransition();
    }
}


