using System;

namespace DosinisSDK.Game
{
    public interface IStagedGame : IGame
    {
        void LoadStage(int id);
        void NextStage();
        void RestartStage();

        event Action<Stage> OnStageLoaded;
        Stage CurrentStage { get; }
        int CurrentStageId { get; }
    }
}
