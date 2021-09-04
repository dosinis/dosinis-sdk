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

        public GameElement CreateGameElement(GameElement gameElement, Vector3 position)
        {
            if (gameElement == null)
            {
                LogError("Trying to create GameElement which source is null");
                return null;
            }

            var instance = Instantiate(gameElement, transform);
            instance.gameObject.transform.position = position;
            instance.Init(this);
            gameElements.Add(instance);

            return instance;
        }

        public GameElement CreateGameElement(GameObject source, Vector3 position)
        {
            if (source == null)
            {
                LogError("Trying to create GameElement which source is null");
                return null;
            }

            var gameElement = source.GetComponent<GameElement>();

            if (gameElement)
            {
                return CreateGameElement(gameElement, position);
            }
            
            LogError($"Source {source.name} doesn't have GameElement! Have you forgot to assign it?");
            return null;
        }

        public void DestroyGameElement(GameElement element)
        {
            if (element == null)
            {
                LogError("Trying to destroy game element which is null");
                return;
            }

            gameElements.Remove(element);
            element.Destruct();
        }
    }
}
