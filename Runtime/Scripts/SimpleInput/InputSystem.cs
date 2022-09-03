using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DosinisSDK.SimpleInput
{
    public class InputSystem : MonoBehaviour
    {
        [SerializeField]
        private InputHandler axisInput; //? Should be an array? Because users might want to change in some games from joystick to buttons and etc

        [SerializeField] private InputHandler rotationInput;
        [SerializeField] private float sensitivity = 10;

        // Solution for "interactInput" - there might be more than one interact option. I.e. Button "E", button "F" and their counterparts on GUI
        // So basically use the KeyCode enum here
        // GUI buttons that inherit or has/is a "KeyCodeButton" can chose what KeyCode on click it would represent.
        // And then basically 

        public static Vector2 Axis { get; private set; }
        public static Vector2 Rotation { get; private set; }
        public static bool Enabled { get; set; } = true;

        // Private 

        private readonly List<KeyCode> pressedKeys = new List<KeyCode>();

        private KeyCode[] keyCodes;

        private static Camera cam;

        private static InputSystem instance;

        // Static ref

        private void Awake()
        {
            instance = this;
            keyCodes = Enum.GetValues(typeof(KeyCode)) as KeyCode[];
            cam = Camera.main;
        }

        private void Update()
        {
            if (Enabled == false)
                return;

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            if (Mathf.Approximately(horizontal, 0) == false || Mathf.Approximately(vertical, 0) == false)
            {
                var input = new Vector2(horizontal, vertical);

                if (input.sqrMagnitude > 1)
                {
                    input.Normalize();
                }

                Axis = Vector2.Lerp(Axis, input, sensitivity * Time.deltaTime);
            }
            else
            {
                if (axisInput) // Iterate through InputHandler[]
                {
                    Axis = Vector2.Lerp(Axis, axisInput.Direction, sensitivity * Time.deltaTime);

                    if (axisInput.Direction == Vector2.zero &&
                        Axis.sqrMagnitude < 0.01f) // consider it's zero already at this point
                    {
                        Axis = Vector2.zero;
                    }
                }
                else
                {
                    Axis = Vector2.Lerp(Axis, Vector2.zero, sensitivity * Time.deltaTime);

                    if (Axis.sqrMagnitude < 0.01f) // consider it's zero already at this point
                    {
                        Axis = Vector2.zero;
                    }
                }
            }

            if (rotationInput)
            {
                Rotation = rotationInput.Direction;
            }
        }

        public static bool GetKey(KeyCode keyCode)
        {
            KeyCode pressedKey = KeyCode.None;

            foreach (KeyCode kcode in instance.keyCodes)
            {
                if (Input.GetKey(kcode))
                {
                    pressedKey = kcode;
                }
            }

            // TODO: iterate through "KeyCodeButton" and check whether it was pressed

            return pressedKey == keyCode;
        }

        public static bool GetKeyDown(KeyCode keyCode)
        {
            KeyCode pressedKey = KeyCode.None;

            foreach (KeyCode kcode in instance.keyCodes)
            {
                if (Input.GetKeyDown(kcode))
                {
                    pressedKey = kcode;
                }
            }

            // TODO: iterate through "KeyCodeButton" and check whether it was pressed

            return pressedKey == keyCode;
        }

        // TODO: GetKeyUp(KeyCode keyCode)

        // Helpers

        private static bool IsPointerOverUI()
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
        
        public static bool GetMouseWorldPoint(out Vector3 worldPoint)
        {
            if (Input.GetMouseButton(0) && IsPointerOverUI() == false)
            {
                RaycastHit hit;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    worldPoint = hit.point;
                    return true;
                }
            }

            worldPoint = Vector3.zero;
            return false;
        }
    }
}