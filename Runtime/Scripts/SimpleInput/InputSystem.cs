using System;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.SimpleInput
{
    // Combine various types of input into one, unified Input system.

    public class InputSystem : MonoBehaviour
    {
        [SerializeField] private InputHandler axisInput; // Should be an array? Because users might want to change in some games from joystick to buttons and etc
        [SerializeField] private InputHandler rotationInput;

        // Solution for "interactInput" - there might be more than one interact option. I.e. Button "E", button "F" and their counterparts on GUI
        // So basically use the KeyCode enum here
        // GUI buttons that inherit or has/is a "KeyCodeButton" can chose what KeyCode on click it would represent.
        // And then basically 

        public static Vector2 Axis { get; private set; }
        public static Vector2 Rotation { get; private set; }

        // Private 

        private readonly List<KeyCode> pressedKeys = new List<KeyCode>();

        private KeyCode[] keyCodes;

        // Static ref

        private static InputSystem instance;

        private void Awake()
        {
            keyCodes = Enum.GetValues(typeof(KeyCode)) as KeyCode[];
            instance = this;
        }

        private void Update()
        {
            if (axisInput) // Iterate through InputHandler[] 
            {
                Axis = axisInput.Direction;
            }

            if (Axis == Vector2.zero)
            {
                float horizontal = UnityEngine.Input.GetAxis("Horizontal");
                float vertical = UnityEngine.Input.GetAxis("Vertical");

                Axis = new Vector2(horizontal, vertical);
            }

            if (rotationInput) // Iterate through InputHandler[] 
            {
                Rotation = rotationInput.Direction;
            }
            else // For now enabling only one rotational device. Gotta think how to implement multiple rotational input support
            {
                float rotationX = UnityEngine.Input.GetAxis("Mouse X");
                float rotationY = UnityEngine.Input.GetAxis("Mouse Y");

                Rotation = new Vector2(rotationX, rotationY);
            }
        }

        public static bool GetKey(KeyCode keyCode)
        {
            KeyCode pressedKey = KeyCode.None;

            foreach (KeyCode kcode in instance.keyCodes)
            {
                if (UnityEngine.Input.GetKey(kcode))
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
                if (UnityEngine.Input.GetKeyDown(kcode))
                {
                    pressedKey = kcode;
                }
            }

            // TODO: iterate through "KeyCodeButton" and check whether it was pressed

            return pressedKey == keyCode;
        }

        // TODO: GetKeyUp(KeyCode keyCode)
    }
}
