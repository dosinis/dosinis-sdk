using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Game
{
    public class Stage : Managed
    {
        [SerializeField] protected StageConfig mainConfig;

        public IStagedGame stagedGame { get; private set; }
        public int Index { get; private set; }

        public override void Init(ISceneManager game)
        {
            base.Init(game);

            stagedGame = game as IStagedGame;
            Index = stagedGame.CurrentStageId;
        }
    }
}
