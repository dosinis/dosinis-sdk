using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Core
{
    public interface IManaged
    {
        void OnInit();
        ISceneManager SceneManager { get; }
    }
}

