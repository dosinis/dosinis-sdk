using DosinisSDK.Core;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DosinisSDK.Game
{
    public class StagedGame : Game, IStagedGame
    {
        private StagedGameConfig config;

        private StagedGameData data;

        private bool interrupted = false;

        public Stage CurrentStage { get; private set; }
        public int CurrentStageId => data.stageId;

        public event Action<Stage> OnStageLoaded = stage => { };

        public event Action<Stage> OnStageCompleted = stage => { };

        public event Action<Stage> OnStageFailed = stage => { };

        public override void Init(IApp app)
        {
            base.Init(app);

            config = GetConfigAs<StagedGameConfig>();

            var dataManager = app.GetCachedModule<IDataManager>();

            data = dataManager.LoadData<StagedGameData>();

            dataManager.RegisterData(data);

            LoadStage(data.stageId);
        }

        public override void Process(float delta)
        {
            if (interrupted)
                return;

            base.Process(delta);
        }

        public void LoadStage(int id)
        {
            if (CurrentStage)
            {
                DestroyGameElement(CurrentStage);
            }

            if (config.stages.Length <= id)
            {
                id = Random.Range(Mathf.FloorToInt(config.stages.Length / 2), config.stages.Length);
            }

            CurrentStage = CreateGameElement(config.stages[id], Vector3.zero) as Stage;

            OnStageLoaded(CurrentStage);
        }

        public void LoadStage()
        {
            LoadStage(data.stageId);
            interrupted = false;
        }

        public virtual void CompleteStage()
        {
            interrupted = true;
            data.stageId++;
            OnStageCompleted(CurrentStage);
        }

        public virtual void FailStage()
        {
            interrupted = true;
            OnStageFailed(CurrentStage);
        }

        public StageElement CreateStageElement(StageElement gameElement, Vector3 position)
        {
            if (gameElement == null)
            {
                LogError("Trying to create StageElement which source is null");
                return null;
            }

            return CreateGameElement(gameElement, position, CurrentStage.transform) as StageElement;
        }

        public StageElement CreateStageElement(GameObject source, Vector3 position)
        {
            if (source == null)
            {
                LogError("Trying to create GameElement which source is null");
                return null;
            }

            var gameElement = source.GetComponent<StageElement>();

            if (gameElement)
            {
                return CreateStageElement(gameElement, position);
            }

            LogError($"Source {source.name} doesn't have StageElement! Have you forgot to assign it?");
            return null;
        }
    }
}
