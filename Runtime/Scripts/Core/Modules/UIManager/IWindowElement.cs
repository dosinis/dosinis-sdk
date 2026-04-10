using System;

namespace DosinisSDK.Core
{
    public interface IWindowElement : IDisposable
    {
        string name { get; }
        public void Init(IApp app, IWindow parentWindow);
        public void Show(Action done = null);
        public void Hide();
    }

    public interface IWindowElementWithArgs<TArgs> : IWindowElement
    {
        public void Show(TArgs args, Action done = null);
    }
}