using UnityEngine;

namespace DosinisSDK.Game
{
    public class StageElement : GameElement
    {
        protected Stage stage;

        public override void Init(IGame game)
        {
            base.Init(game);

            var stagedGame = game as IStagedGame;

            stage = stagedGame.CurrentStage;
        }
    }
}
