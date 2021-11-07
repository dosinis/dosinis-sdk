using DosinisSDK.Core;
using System;

namespace DosinisSDK.RateUs
{
    public interface IRateUsManager : IModule
    {
        event Action OnInitRating;
        bool IsRated { get; }
        void Rate(bool dummyRate = false);
        void InitRating();
    }
}