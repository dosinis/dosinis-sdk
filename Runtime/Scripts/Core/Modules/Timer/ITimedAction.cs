namespace DosinisSDK.Core
{
    public interface ITimedAction
    {
        ITimedAction Start();
        ITimedAction Stop();
    }
}
