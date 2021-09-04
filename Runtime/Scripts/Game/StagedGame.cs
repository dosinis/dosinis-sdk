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

        public Stage CurrentStage { get; private set; }
        public int CurrentStageId => data.stageId;

        public event Action<Stage> OnStageLoaded = stage => { };

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
            base.Process(delta);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                NextStage();
            }
        }

        public void LoadStage(int id)
        {
            if (CurrentStage)
            {
                DestroyGameElement(CurrentStage);
            }
            
            if (config.stages.Length < id)
            {
                id = Random.Range(Mathf.FloorToInt(config.stages.Length / 2), config.stages.Length);
            }

            CurrentStage = CreateGameElement(config.stages[id], Vector3.zero) as Stage;

            OnStageLoaded(CurrentStage);
        }

        public void NextStage()
        {
            data.stageId++;
            LoadStage(data.stageId);
        }

        public void RestartStage()
        {
            LoadStage(data.stageId);
        }
    }
}
