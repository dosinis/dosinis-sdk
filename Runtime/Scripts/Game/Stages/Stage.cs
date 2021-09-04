using UnityEngine;

namespace DosinisSDK.Game
{
    public class Stage : GameElement
    {
        [SerializeField] protected StageConfig mainConfig;

        protected IStagedGame game;

        public int Index { get; private set; }

        public override void Init(IGame game)
        {
            base.Init(game);

            this.game = game as IStagedGame;
            Index = this.game.CurrentStageId;
        }
    }
}
