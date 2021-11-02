using UnityEngine;

namespace DosinisSDK.UI
{
    public class BounceLoop : MonoBehaviour
    {
        [SerializeField] private float intensity = 1.1f;
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private AnimationCurve bounceCurve = AnimationCurve.Linear(0, 0, 1, 1);

        private float timer;
        private Vector3 initScale;
        private bool reversing = false;

        private void Awake()
        {
            initScale = transform.localScale;
        }

        private void Update()
        {
            if (timer < duration && reversing == false)
            {
                timer += Time.deltaTime;
            }
            else
            {
                reversing = true;

                timer -= Time.deltaTime;

                if (timer < 0)
                {
                    reversing = false;
                    timer = 0;
                }
            }

            transform.localScale = Vector3.Lerp(initScale, initScale * intensity, bounceCurve.Evaluate(timer / duration));
        }
    }
}
