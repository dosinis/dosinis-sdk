using System;
using UnityEngine;

namespace DosinisSDK.Utils
{
    [RequireComponent(typeof(Animator))]
    public class ManualAnimator : MonoBehaviour
    {
        // NOTE: Possible use of AnimationClip instead
        [SerializeField] private string motionTimeKey;

        private Animator animator;

        public event Action OnAnimationEvent = () => { };

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Process(float normalizedTime)
        {
            animator.SetFloat(motionTimeKey, normalizedTime);
        }

        // Assign this as event at desired animation frame (in Animator)
        public void AnimationEvent()
        {
            OnAnimationEvent();
        }
    }
}
