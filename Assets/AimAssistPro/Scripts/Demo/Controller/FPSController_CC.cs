using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.AimAssists;
using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Helper.Chaining;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
#endif

namespace Agoston_R.Aim_Assist_Pro.Scripts.Demo.Controller
{
    /// <summary>
    /// Controller based on Unity's Input System controller script with the Magnetism assist integrated.
    ///
    /// See the highlighted comments to show you how to integrate the aim assist.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
#endif
    public class FPSController_CC : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 4.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 6.0f;

        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 1.0f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.1f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")] public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.5f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 90.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;

        // cinemachine
        private float cinemachineTargetPitch;

        // player
        private float speed;
        private float rotationVelocity;
        private float verticalVelocity;
        private float terminalVelocity = 53.0f;

        // timeout deltatime
        private float jumpTimeoutDelta;
        private float fallTimeoutDelta;

        private CharacterController controller;
        private InputHandler input;
        private GameObject mainCamera;

        // GET A REFERENCE TO THE AIM ASSIST SCRIPT
        private Magnetism magnetism;
        private AimLock aimLock;
        private AimEaseIn aimEaseIn;
        private AutoAim autoAim;
        private PrecisionAim precisionAim;

        // YOU CAN USE THIS IF YOU HAVE MULTIPLE AIM ASSISTS AT USE THAT WORK BASED ON SCALING YOUR LOOK INPUT.
        private readonly LookInputBasedAimAssistChainer lookAimAssistChainer = new LookInputBasedAimAssistChainer();

        private Shooter shooter;

        private const float _threshold = 0.01f;

        private void Awake()
        {
            // get a reference to our main camera
            if (mainCamera == null)
            {
                mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            magnetism = GetComponent<Magnetism>();
            aimLock = GetComponent<AimLock>();
            aimEaseIn = GetComponent<AimEaseIn>();
            autoAim = GetComponent<AutoAim>();
            precisionAim = GetComponent<PrecisionAim>();
            shooter = GetComponent<Shooter>();
        }

        private void Start()
        {
            controller = GetComponent<CharacterController>();
            input = GetComponent<InputHandler>();

            // reset our timeouts on start
            jumpTimeoutDelta = JumpTimeout;
            fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void FixedUpdate()
        {
            if (!shooter)
            {
                return;
            }

            shooter.Trigger = input.shoot;
        }

        private void LateUpdate()
        {
            CameraRotation();
            UseMagnetism();
            UseAimLock();
        }

        private void UseAimLock()
        {
            // CALL THE SCRIPT'S ASSISTING METHOD
            var aimLockResult = aimLock.AssistAim();

            // ROTATE YOUR PLAYER BY ADDING THE TURN ADJUSTMENT TO YOUR ROTATION ALONG THE Y AXIS - TURN SIDEWAYS.
            transform.Rotate(aimLockResult.TurnAddition);

            // ADD THE ASSISTED PITCH TO YOUR TARGET PITCH AND CLAMP IT, THEN SET CINEMACHINE'S PITCH TO IS, JUST LIKE WHEN HANDLING USER INPUT.
            cinemachineTargetPitch += aimLockResult.PitchAdditionInDegrees;
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0f, 0f);
        }

        private void UseMagnetism()
        {
            // CALL THE ASSISTING METHOD AND PASS IT THE NECESSARY INPUT
            var magnetismResult = magnetism.AssistAim(input.move);

            // ROTATE YOUR PLAYER, MUCH THE SAME AS FOR HANDLING PLAYER INPUT
            transform.Rotate(magnetismResult.TurnAddition);

            // ADD THE PITCH ADJUSTMENT TO YOUR TARGET PITCH THEN CLAMP IT AND SET CINEMACHINE TO THAT PITCH, SAME AS HANDLING USER INPUT.
            cinemachineTargetPitch += magnetismResult.PitchAdditionInDegrees;
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0f, 0f);
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            if (input.look.sqrMagnitude < _threshold)
            {
                return;
            }

            // USING THE LOOK INPUT AIM ASSIST CHAINER TO USE MULTIPLE AIM ASSISTS.
            // WITHOUT IT FOR A SINGLE AIM ASSIST YOU COULD JUST WRITE E.G.
            // var look = autoAim.AssistAim(input.look);

            var look = lookAimAssistChainer
                .WithLookInputDelta(input.look)
                .UsingAimEaseIn(aimEaseIn)
                .UsingPrecisionAim(precisionAim)
                .UsingAutoAim(autoAim)
                .GetModifiedLookInputDelta();

            cinemachineTargetPitch += look.y * RotationSpeed * Time.deltaTime;
            rotationVelocity = look.x * RotationSpeed * Time.deltaTime;

            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0.0f, 0.0f);
            transform.Rotate(Vector3.up * rotationVelocity);
        }

        private void Move()
        {
            float targetSpeed = input.sprint ? SprintSpeed : MoveSpeed;

            if (input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = input.analogMovement ? input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                speed = Mathf.Round(speed * 1000f) / 1000f;
            }
            else
            {
                speed = targetSpeed;
            }

            Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;

            if (input.move != Vector2.zero)
            {
                inputDirection = transform.right * input.move.x + transform.forward * input.move.y;
            }

            controller.Move(inputDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                fallTimeoutDelta = FallTimeout;

                if (verticalVelocity < 0.0f)
                {
                    verticalVelocity = -2f;
                }

                if (input.jump && jumpTimeoutDelta <= 0.0f)
                {
                    verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }

                if (jumpTimeoutDelta >= 0.0f)
                {
                    jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                jumpTimeoutDelta = JumpTimeout;

                if (fallTimeoutDelta >= 0.0f)
                {
                    fallTimeoutDelta -= Time.deltaTime;
                }

                input.jump = false;
            }

            if (verticalVelocity < terminalVelocity)
            {
                verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }
    }
}
