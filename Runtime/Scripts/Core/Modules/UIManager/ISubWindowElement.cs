using System;

namespace DosinisSDK.Core
{
    public interface ISubWindowElement : IDisposable
    {
        public void Init(IApp app, IWindow parentWindow);
        public void Show(Action done = null);
        public void Hide();
    }

    public interface ISubWindowElementWithArgs<TArgs> : ISubWindowElement
    {
        public void Show(TArgs args, Action done = null);
    }
}