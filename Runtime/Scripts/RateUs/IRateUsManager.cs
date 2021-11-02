using DosinisSDK.Core;

namespace DosinisSDK.RateUs
{
    public interface IRateUsManager : IModule
    {
        bool IsRated { get; }
        void Rate();
    }
}