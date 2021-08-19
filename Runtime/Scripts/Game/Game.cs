using DosinisSDK.Core;
using System.Collections.Generic;

namespace DosinisSDK.Game
{
    public class Game : BehaviourModule, IGame
    {
        private readonly List<GameElement> gameElements = new List<GameElement>();

        public override void Init(IApp app)
        {
            foreach (var ge in GetComponentsInChildren<GameElement>(true))
            {
                ge.Init(this);
                gameElements.Add(ge);
            }
        }

        public override void Process(float delta)
        {
            foreach (var ge in gameElements)
            {
                if (ge.Alive)
                {
                    ge.Process(delta);
                }
            }
        }

        public void CreateGameElement()
        {
           
        }

        public void ReturnElementToPool(GameElement element)
        {
            element.gameObject.SetActive(false);
        }
    }
}
