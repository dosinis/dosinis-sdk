namespace DosinisSDK.Core
{
    public interface IFixedProcessable
    {
        void FixedProcess(in float fixedDelta);
    }
}
