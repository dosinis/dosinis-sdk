using DosinisSDK.Core;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DosinisSDK.Game
{
    public class StagedGame : MonoBehaviour, IStagedGame
    {
        [SerializeField] private StagedGameConfig config;

        private StagedGameData data;

        private bool interrupted = false;

        // Properties

        public Stage CurrentStage { get; private set; }
        public int CurrentStageId => data.stageId;

        // Events

        public event Action<Stage> OnStageLoaded;

        public event Action<Stage> OnStageCompleted;

        public event Action<Stage> OnStageFailed;

        //public override void OnInit()
        //{
        //    data = App.Core.GetCachedModule<IDataManager>().RetrieveOrCreateData<StagedGameData>();

        //    LoadStage(data.stageId);
        //}

        //public override void Process(float delta)
        //{
        //    if (interrupted)
        //        return;
        //}

        public void LoadStage(int id)
        {
            if (CurrentStage)
            {
                Destroy(CurrentStage);
            }

            if (config.stages.Length <= id)
            {
                id = Random.Range(Mathf.FloorToInt(config.stages.Length / 2), config.stages.Length);
            }

            CurrentStage = Instantiate(config.stages[id], transform);
            CurrentStage.transform.position = Vector3.zero;

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
