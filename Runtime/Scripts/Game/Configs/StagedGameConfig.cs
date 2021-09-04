using DosinisSDK.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Game
{
    [CreateAssetMenu(fileName = "StagedGameConfig", menuName = "DosinisSDK/Configs/Game/StagedGameConfig")]
    public class StagedGameConfig : ModuleConfig
    {
        public Stage[] stages;
    }
}
