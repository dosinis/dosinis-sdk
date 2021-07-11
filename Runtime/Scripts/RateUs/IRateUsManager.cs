using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.RateUs
{
    public interface IRateUsManager
    {
        void Init();
        bool IsRated { get; }
        void Rate();

    }
}