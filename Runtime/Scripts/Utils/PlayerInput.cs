using System;
using UnityEngine;

namespace DosinisSDK.Utils
{
    public class PlayerInput : MonoBehaviour
    {
        public static event Action<Vector2> OnClickHappened = pos => { };

        [SerializeField] private Camera cam;

        private void Awake()
        {
            if (cam == null)
            {
                cam = Camera.main;
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var pos = cam.ScreenToWorldPoint(Input.mousePosition);
                OnClickHappened(pos);
            }
        }
    }
}

