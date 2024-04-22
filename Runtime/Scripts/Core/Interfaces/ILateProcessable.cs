namespace DosinisSDK.Core
{
    public interface ILateProcessable
    {
        void LateProcess(in float delta);
    }
}
