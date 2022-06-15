using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DosinisSDK.SimpleInput
{
    public class PlayerInput : MonoBehaviour
    {
        public static event Action<Vector2> OnClickHappened;

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
            if (Input.GetMouseButtonDown(0) && IsPointerOverUI() == false)
            {
                var pos = cam.ScreenToWorldPoint(Input.mousePosition);
                OnClickHappened?.Invoke(pos);
            }
        }
        
        private bool IsPointerOverUI()
        {
            if (Application.isEditor == false && Input.touchSupported)
            {
                if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    return true;
                }
            }

            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}

