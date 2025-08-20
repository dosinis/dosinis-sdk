namespace DosinisSDK.Core
{
    public class CoreEvents
    {
        public class WindowOpenedEvent : IEvent
        {
            public readonly IWindow window;
        
            public WindowOpenedEvent(IWindow window)
            {
                this.window = window;
            }
        }
    
        public class OnWindowClosedEvent : IEvent
        {
            public readonly IWindow window;
        
            public OnWindowClosedEvent(IWindow window)
            {
                this.window = window;
            }
        }
    }
}
