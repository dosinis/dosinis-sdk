using System;
using UnityEngine;

namespace DosinisSDK.Game
{
    public interface IStagedGame : IGame
    {
        void LoadStage(int id);
        void LoadStage();
        void CompleteStage();
        void FailStage();
        event Action<Stage> OnStageLoaded;
        event Action<Stage> OnStageCompleted;
        event Action<Stage> OnStageFailed;
        Stage CurrentStage { get; }
        int CurrentStageId { get; }
        StageElement CreateStageElement(StageElement gameElement, Vector3 position);
        StageElement CreateStageElement(GameObject source, Vector3 position);
    }
}
