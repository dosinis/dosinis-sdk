namespace DosinisSDK.UI.Elements
{
    public abstract class ElementFor<T> : Element
    {
        public abstract void Setup(T args);
    }
}