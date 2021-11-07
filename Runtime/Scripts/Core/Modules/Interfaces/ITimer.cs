using System;

namespace DosinisSDK.Core
{
    public interface ITimer : IModule
    {
        void Delay(float delay, Action done);
        void SkipFrame(Action done);
    }
}
