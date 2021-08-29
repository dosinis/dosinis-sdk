using DosinisSDK.Core;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Game
{
    public class Game : BehaviourModule, IGame, IProcessable
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

        public virtual void Process(float delta)
        {
            foreach(var gameElement in gameElements)
            {
                if (gameElement.Alive)
                {
                    gameElement.Process(delta);
                }
            }
        }

        public void CreateGameElement(GameObject source, Vector3 position)
        {
            if (source == null)
            {
                LogError("Trying to create GameElement which source is null");
                return;
            }

            var gameElement = source.GetComponent<GameElement>();

            if (gameElement)
            {
                var instance = Instantiate(gameElement, transform);
                instance.gameObject.transform.position = position;
                instance.Init(this);
                gameElements.Add(instance);
            }
            else
            {
                LogError($"Source {source.name} doesn't have GameElement! Have you forgot to assign it?");
            }
        }

        public void DestroyGameElement(GameElement element)
        {
            gameElements.Remove(element);
            element.Destruct();
        }
    }
}
