using System;
using UnityEngine;

namespace DosinisSDK.Utils
{
    public class PlayerInput : MonoBehaviour
    {
        public static event Action<Vector2> OnClickHappened = pos => { };

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnClickHappened(Input.mousePosition);
            }
        }
    }
}

