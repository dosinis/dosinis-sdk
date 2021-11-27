using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Game
{
    public class Stage : Managed
    {
        [SerializeField] protected StageConfig mainConfig;

        public IStagedGame stagedGame { get; private set; }
        public int Index { get; private set; }

        public override void OnInit()
        {
            //stagedGame = SceneManager as IStagedGame;

            //Index = stagedGame.CurrentStageId;
        }
    }
}
