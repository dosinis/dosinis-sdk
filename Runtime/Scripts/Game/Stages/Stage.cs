using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Game
{
    public class Stage : GameElement
    {
        [SerializeField] protected StageConfig mainConfig;

        private readonly HashSet<StageElement> stageElements = new HashSet<StageElement>();

        public IStagedGame stagedGame { get; private set; }

        public int Index { get; private set; }

        public override void Init(IGame game)
        {
            base.Init(game);

            stagedGame = game as IStagedGame;
            Index = stagedGame.CurrentStageId;
        }

        public override void Process(float delta)
        {
            base.Process(delta);

            foreach (var se in stageElements)
            {
                if (se.Alive)
                    se.Process(delta);
            }
        }

        public StageElement CreateStageElement(StageElement element, Vector3 position)
        {
            if (element == null)
            {
                Debug.LogError("Trying to create StageElement which is null");
                return null;
            }

            var instance = Instantiate(element, transform);
            instance.gameObject.transform.position = position;
            instance.Init(this);
            stageElements.Add(instance);

            return instance;
        }

        public StageElement CreateStageElement(GameObject source, Vector3 position)
        {
            if (source == null)
            {
                Debug.LogError("Trying to create StageElement which source is null");
                return null;
            }

            var gameElement = source.GetComponent<StageElement>();

            if (gameElement)
            {
                return CreateStageElement(gameElement, position);
            }

            Debug.LogError($"Source {source.name} doesn't have StageElement! Have you forgot to assign it?");
            return null;
        }

        public void DestroyStageElement(StageElement element)
        {
            if (element && stageElements.Contains(element))
            {
                stageElements.Remove(element);
                element.Destruct();
            }
        }
    }
}
