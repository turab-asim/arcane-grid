using Agoston_R.Aim_Assist_Pro.Scripts.Demo.Menu;
using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
namespace Agoston_R.Aim_Assist_Pro.Scripts.Demo.Controller
{
    /// <summary>
    /// Example input handler for the aim assist demos.
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        private const float AxisThreshold = 0.1f;

        [Header("Settings for input manager")] [Tooltip("Sensitivity when using input manager")]
        public float inputManagerLookSensitivity = 70f;

        [Header("Character Input Values")] public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool shoot;

        [Header("Movement Settings")] public bool analogMovement;

        private MenuController menuController;

        private void Start()
        {
            menuController = FindObjectOfType<MenuController>();
        }

#if ENABLE_INPUT_SYSTEM

        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            LookInput(value.Get<Vector2>());
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }

        public void OnShoot(InputValue value)
        {
            ShootInput(value.isPressed);
        }

        public void OnMenu()
        {
            menuController.HandlePauseMenu();
        }
#else

        private void Update()
        {
            HandleMovement();
            HandleLook();
            HandleSprint();
            HandleJump();
            HandleFire();
            HandleMenu();
        }

        private void HandleMenu()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menuController.HandlePauseMenu();
            }
        }

        private void HandleFire()
        {
            ShootInput(TryGetAxis("Right Trigger") > AxisThreshold || TryGetAxis("Fire1") > AxisThreshold);
        }

        private void HandleJump()
        {
            JumpInput(Input.GetButtonDown("Jump"));
        }

        private void HandleSprint()
        {
            SprintInput(TryGetAxis("Left Trigger") > AxisThreshold || Input.GetKey(KeyCode.LeftShift));
        }

        private void HandleLook()
        {
            // vector's X is the horizontal turn, vector's Y is the vertical pitch.
            var lookInput = new Vector2(TryGetAxis("Mouse X") * inputManagerLookSensitivity, - TryGetAxis("Mouse Y") * inputManagerLookSensitivity);
            LookInput(lookInput);
        }

        private void HandleMovement()
        {
            var moveInput = new Vector2(TryGetAxis("Horizontal"), TryGetAxis("Vertical"));
            MoveInput(moveInput);
        }

        private float TryGetAxis(string name)
        {
            try
            {
                return Input.GetAxis(name);
            }
            catch
            {
                return 0;
            }
        }

#endif


        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        public void ShootInput(bool newShootState)
        {
            shoot = newShootState;
        }
    }
}