using UnityEngine;

namespace DosinisSDK.Game
{
    public class Stage : GameElement
    {
        [SerializeField] protected StageConfig mainConfig;

        protected IStagedGame stagedGame;

        public int Index { get; private set; }

        public override void Init(IGame game)
        {
            base.Init(game);

            stagedGame = game as IStagedGame;
            Index = stagedGame.CurrentStageId;
        }
    }
}
