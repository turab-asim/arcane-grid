using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.AimAssists;
using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Helper.Chaining;
using UnityEngine;

namespace Agoston_R.Aim_Assist_Pro.Scripts.Demo.Controller
{
    /// <summary>
    /// Controller based on Unity's Input System controller script with the Magnetism assist integrated.
    ///
    /// See the highlighted comments to show you how to integrate the aim assist.
    /// </summary>
    public class FPSController_RB : MonoBehaviour
    {
        private const float Threshold = 0.01f;

        [Header("Player")] [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 4.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 6.0f;

        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 1.0f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Space(10)] [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)] [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.1f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")] [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")] public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.5f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")] [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 90.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;

        private float cinemachineTargetPitch;
        private float speed;
        private float rotationVelocity;
        private float jumpTimeoutDelta;
        private float fallTimeoutDelta;
        private InputHandler input;
        private Rigidbody rb;

        // GET A REFERENCE TO THE AIM ASSIST SCRIPT
        private Magnetism magnetism;
        private AimLock aimLock;
        private AimEaseIn aimEaseIn;
        private AutoAim autoAim;
        private PrecisionAim precisionAim;

        // YOU CAN USE THIS IF YOU HAVE MULTIPLE AIM ASSISTS AT USE THAT WORK BASED ON SCALING YOUR LOOK INPUT.
        private readonly LookInputBasedAimAssistChainer lookAimAssistChainer = new LookInputBasedAimAssistChainer();

        private Shooter shooter;

        private void Awake()
        {
            input = GetComponent<InputHandler>();
            magnetism = GetComponent<Magnetism>();
            aimLock = GetComponent<AimLock>();
            aimEaseIn = GetComponent<AimEaseIn>();
            autoAim = GetComponent<AutoAim>();
            precisionAim = GetComponent<PrecisionAim>();
            rb = GetComponent<Rigidbody>();
            shooter = GetComponent<Shooter>();
        }

        private void FixedUpdate()
        {
            JumpAndGravity();
            GroundedCheck();
            Move();
            Shoot();
        }
        
        private void Shoot()
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
            // get the assisted angles
            var aimAssist = aimLock.AssistAim();

            // add turn addition - NOTE THAT MoveRotation SETS WHERE YOUR RIGIDBODY LOOKS. PRESERVE YOUR ROTATION AFTER INPUT BY MULTIPLYING THE ADDITION WITH YOUR ORIGINAL ROTATION FIRST.
            var turnAddition = Quaternion.Euler(aimAssist.TurnAddition);
            rb.MoveRotation(rb.rotation * turnAddition);

            // add pitch addition
            cinemachineTargetPitch += aimAssist.PitchAdditionInDegrees;
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0f, 0f);
        }

        private void UseMagnetism()
        {
            // get the assisted angles, using the player MOVEMENT input as parameter
            var aimAssist = magnetism.AssistAim(input.move);
            
            // add turn addition
            var turnAddition = Quaternion.Euler(aimAssist.TurnAddition);
            rb.MoveRotation(rb.rotation * turnAddition);

            // add pitch addition
            cinemachineTargetPitch += aimAssist.PitchAdditionInDegrees;
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0f, 0f);
        }

        private void GroundedCheck()
        {
            var pos = transform.position;
            Vector3 spherePosition = new Vector3(pos.x, pos.y - GroundedOffset, pos.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            if (input.look.sqrMagnitude < Threshold)
            {
                rb.angularVelocity = Vector3.zero;
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

            var rotation = Quaternion.Euler(Vector3.up * rotationVelocity);
            rb.angularVelocity = Vector3.zero;
            rb.MoveRotation(rb.rotation * rotation);
        }

        private void Move()
        {
            var targetSpeed = CalculateTargetSpeed();
            var inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;

            if (input.move != Vector2.zero)
            {
                inputDirection = transform.right * input.move.x + transform.forward * input.move.y;
            }

            var speedOffset = 0.1f;
            var inputMagnitude = input.analogMovement ? input.move.magnitude : 1f;
            var currentHorizontalSpeed = new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.fixedDeltaTime * SpeedChangeRate);
                speed = Mathf.Round(speed * 1000f) / 1000f;
            }
            else
            {
                speed = targetSpeed;
            }

            var targetVelocity = inputDirection * speed;
            rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
        }

        private float CalculateTargetSpeed()
        {
            var targetSpeed = input.sprint ? SprintSpeed : MoveSpeed;

            if (input.move == Vector2.zero)
            {
                targetSpeed = 0.0f;
            }

            return targetSpeed;
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                fallTimeoutDelta = FallTimeout;

                if (input.jump && jumpTimeoutDelta <= 0.0f)
                {
                    var vel = rb.velocity;
                    rb.velocity = new Vector3(vel.x, CalculateJumpVelocity(), vel.z);
                }

                if (jumpTimeoutDelta >= 0.0f)
                {
                    jumpTimeoutDelta -= Time.fixedDeltaTime;
                }
            }
            else
            {
                jumpTimeoutDelta = JumpTimeout;

                if (fallTimeoutDelta >= 0.0f)
                {
                    fallTimeoutDelta -= Time.fixedDeltaTime;
                }

                input.jump = false;
            }
        }

        private float CalculateJumpVelocity()
        {
            return Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * JumpHeight);
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