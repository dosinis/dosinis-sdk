using System.Collections.Generic;
using DosinisSDK.Inspector;
using UnityEngine;

namespace DosinisSDK.Utils
{
    public abstract class Reward : ScriptableObject, IReward
    {
        [SerializeField, ShowSprite] private Sprite mainIcon;
        [SerializeField] private string id;
        [SerializeField] private string fallbackTitle;
        [SerializeField] private Reward[] extraRewards;

        public IReadOnlyCollection<Reward> ExtraRewards => extraRewards;
        public string Id => id;
        public Sprite MainIcon => mainIcon;

        private void OnValidate()
        {
            foreach (var handler in extraRewards)
            {
                if (handler == this)
                {
                    Debug.LogError("Reward: Extra reward is referencing to itself. That's not allowed");    
                }
            }
        }

        public void GrantReward()
        {
            OnRewarded();
            
            foreach (var handler in extraRewards)
            {
                if (handler == this)
                {
                    Debug.LogError("Reward: Extra reward is referencing to itself");
                    continue;    
                }

                handler.OnRewarded();
            }
        }
        
        protected abstract void OnRewarded();
        public abstract string GetValueString();
        public abstract string GetDescription();

        public virtual string GetTitle()
        {
            return fallbackTitle;
        }
    }

    public interface IReward
    {
        void GrantReward();
        
        IReadOnlyCollection<Reward> ExtraRewards { get; }
        string Id { get; }
        Sprite MainIcon { get; }

        string GetValueString();
        string GetDescription();
        string GetTitle();
    }
}
