using UnityEngine;

namespace DosinisSDK.Game
{
    public class StageElement : Managed
    {
        protected Stage stage;

        public override void Init(ISceneManager game)
        {
            base.Init(game);

            var stagedGame = game as IStagedGame;

            stage = stagedGame.CurrentStage;
        }
    }
}
