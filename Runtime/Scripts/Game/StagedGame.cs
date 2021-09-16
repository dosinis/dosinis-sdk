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
    }
}
