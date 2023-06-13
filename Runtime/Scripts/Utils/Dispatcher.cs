using System;
using System.Collections.Generic;
using System.Threading;
using DosinisSDK.Core;

namespace DosinisSDK.Utils
{
    public class Dispatcher : BehaviourModule, IProcessable
    {
        private Thread mainThread;
        private volatile bool queued = false;
        private List<Action> backlog = new(8);
        private List<Action> actions = new(8);
        
        public bool IsOnMainThread => mainThread == Thread.CurrentThread;

        public void RunAsync(Action action)
        {
            ThreadPool.QueueUserWorkItem(o => action());
        }

        public void RunAsync(Action<object> action, object state)
        {
            ThreadPool.QueueUserWorkItem(o => action(o), state);
        }

        public void RunOnMainThread(Action action)
        {
            lock (backlog)
            {
                backlog.Add(action);
                queued = true;
            }
        }

        protected override void OnInit(IApp app)
        {
        }

        public void Process(in float delta)
        {
            if (mainThread == null)
            {
                mainThread = Thread.CurrentThread;
            }
            if (queued)
            {
                lock (backlog)
                {
                    (actions, backlog) = (backlog, actions);
                    queued = false;
                }

                foreach (var action in actions)
                    action();

                actions.Clear();
            }
        }
    }
}