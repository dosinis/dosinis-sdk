using DosinisSDK.SimpleInput;
using UnityEngine;

namespace DosinisSDK.FirstPerson
{
    [RequireComponent(typeof(CharacterController))]
    public class FPController : MonoBehaviour
    {
        // Extract to config file
        // e.g. there could be configs for mobile controller and standalone

        [SerializeField] private float walkSpeed = 7.5f;
        [SerializeField] private float runSpeed = 11.5f;
        [SerializeField] private float jumpPower = 8.0f;
        [SerializeField] private float gravity = 9.8f;

        [SerializeField] private float lookSpeed = 2.0f;
        [SerializeField] private float lookYLimit = 45.0f;

        private CharacterController charController;
        private new Camera camera;

        private Vector3 moveDirection = Vector3.zero;
        private float camRotation = 0;
        private bool canMove = true;

        private void Awake()
        {
            charController = GetComponent<CharacterController>();
            camera = GetComponentInChildren<Camera>();

            Debug.Assert(camera, $"{nameof(FPController)} camera is null. Did you forget to add it as a child object?");

            // Lock cursor
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }

        private void Update()
        {
            bool isRunning = UnityEngine.Input.GetKey(KeyCode.LeftShift); // Change to SimpleInput
            float forwardSpeed = canMove ? (isRunning ? runSpeed : walkSpeed) * InputSystem.Axis.y : 0;
            float sidewaysSpeed = canMove ? (isRunning ? runSpeed : walkSpeed) * InputSystem.Axis.x : 0;

            float movementDirectionY = moveDirection.y;

            moveDirection = (transform.TransformDirection(Vector3.forward) * forwardSpeed)
                + (transform.TransformDirection(Vector3.right) * sidewaysSpeed);

            if (UnityEngine.Input.GetButton("Jump") && canMove && charController.isGrounded) // Change to SimpleInput
            {
                moveDirection.y = jumpPower;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }

            if (!charController.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }

            charController.Move(moveDirection * Time.deltaTime);

            if (canMove)
            {
                camRotation += -InputSystem.Rotation.y * lookSpeed;
                camRotation = Mathf.Clamp(camRotation, -lookYLimit, lookYLimit);
                camera.transform.localRotation = Quaternion.Euler(camRotation, 0, 0);
                transform.rotation *= Quaternion.Euler(0, InputSystem.Rotation.x * lookSpeed, 0);
            }
        }
    }
}
