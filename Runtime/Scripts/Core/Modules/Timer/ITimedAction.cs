namespace DosinisSDK.Core
{
    public interface ITimedAction
    {
        ITimedAction Start();
        void Stop();
    }
}
