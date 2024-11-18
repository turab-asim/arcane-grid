using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Helper.Controls;
using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Helper.Info;
using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Helper.Numerics;
using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Model;
using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Target;
using UnityEngine;

namespace Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.AimAssists
{
    /// <summary>
    /// Compensates for the player's strafe by loosely following the target while it's still in assist range.
    ///
    /// Requires a RigidBody to be present on the player.
    /// </summary>
    public class Magnetism : AimAssistBase
    {
        #region Player body and controller references

        // Exposed only to be set conveniently via Inspector. Use player physics for cleaner code. 
        [HideInInspector] public PlayerControlType controlType;
        [HideInInspector] public Rigidbody playerBody;
        [HideInInspector] public CharacterController playerController;

        #endregion

        private IPlayerPhysicsInfo playerPhysics;

        [Header("Horizontal strafing compensation")] [Tooltip("A divisor for the player's strafe movement when they are moving away from the target.")] [Min(1.08f)]
        public float horizontalSmoothnessAwayFromTarget = 1.09f;

        [Tooltip(
            "A divisor for the player's strafe movement when they are strafing towards the target. To prevent turning the player away from the target during mirror strafing, it has to be greater than smoothness away from target.")]
        [Min(1.08f)]
        public float horizontalSmoothnessTowardsTarget = 2f;

        [Tooltip(
            "In metres, to avoid stutter when switching immediately between to and away horizontal strafe smoothness this lerps the change over a distance. Has to be less than aim assist radius.")]
        [Min(0.01f)]
        public float lerpDistance = 0.1f;

        [Header("Vertical compensation")] [Tooltip("If enabled the assist will compensate for player jumping by tracking the target vertically, smoothed out by a factor.")]
        public bool verticalCompensation;

        [Tooltip("A divisor for the player's pitch to be compensated against.")] [Min(1.08f)]
        public float verticalSmoothness = 1.15f;

        private void Start()
        {
            SetUpPlayerPhysicsInfo();
        }

        /// <summary>
        /// Calculates adjustments for the player's rotation and camera pitch to compensate against strafing or jumping. 
        /// </summary>
        /// <param name="moveInputDelta">The player's movement input delta you would use as an input for movement controls.</param>
        /// <returns>The angles in degrees to change player rotation and camera pitch.</returns>
        public AimAssistResult AssistAim(Vector2 moveInputDelta)
        {
            var target = Target;
            if (!target || horizontalSmoothnessAwayFromTarget < 1f || !aimAssistEnabled)
            {
                return AimAssistResult.Empty;
            }

            lerpDistance = Mathf.Clamp(lerpDistance, 0.01f, AimAssistRadius);

            var strafeTowardsTarget = IsPlayerMovingTowardsTarget(target, moveInputDelta.x);
            var calculatedHorizontalSmoothness = CalculateHorizontalSmoothness(moveInputDelta, target, strafeTowardsTarget);
            var horizontalAdjustment = !MovementInputZero(moveInputDelta) ? CalculateHorizontalAssist(moveInputDelta, target, calculatedHorizontalSmoothness) : 0f;
            var verticalAdjustment = verticalCompensation && !VerticalVelocityZero() ? CalculateVerticalAssist(target) : 0f;

            return new AimAssistResult(rotationAdditionInDegrees: horizontalAdjustment.Sanitized(), pitchAdditionInDegrees: verticalAdjustment.Sanitized(),
                turnAddition: horizontalAdjustment.Sanitized() * Vector3.up);
        }

        private float CalculateHorizontalSmoothness(Vector2 moveInputDelta, AimAssistTarget target, bool strafeTowardsTarget)
        {
            var targetPos = target.transform.position;
            var cameraRight = PlayerCamera.right;
            var targetDistance = (targetPos - PlayerCamera.transform.position).magnitude;
            var currentPoint = PlayerCamera.forward * targetDistance + PlayerCamera.position;
            var leftEnd = targetPos - cameraRight * (lerpDistance / 2f);
            var rightEnd = targetPos + cameraRight * (lerpDistance / 2f);

            var factor = (currentPoint.x - leftEnd.x) / lerpDistance;
            return currentPoint.x.Between(leftEnd.x, rightEnd.x) ? LerpSmoothness(moveInputDelta, factor) : SelectSmoothness(strafeTowardsTarget);
        }

        private float SelectSmoothness(bool strafeTowardsTarget)
        {
            return strafeTowardsTarget ? horizontalSmoothnessTowardsTarget : horizontalSmoothnessAwayFromTarget;
        }

        private float LerpSmoothness(Vector2 moveInputDelta, float factor)
        {
            return Mathf.Lerp(horizontalSmoothnessAwayFromTarget, horizontalSmoothnessTowardsTarget,
                moveInputDelta.x > 0 ? 1 - factor : factor);
        }

        private bool VerticalVelocityZero()
        {
            return playerPhysics.Velocity.y.EqualsApprox(0f);
        }

        private bool MovementInputZero(Vector2 moveInputDelta)
        {
            return moveInputDelta.EqualsApprox(Vector2.zero);
        }

        private float CalculateVerticalAssist(AimAssistTarget target)
        {
            return CalculateAssistPerAxis(target, PlayerCamera.right) / verticalSmoothness * Mathf.Sign(playerPhysics.Velocity.y);
        }

        private float CalculateHorizontalAssist(Vector2 moveInputDelta, AimAssistTarget target, float smoothness)
        {
            return CalculateAssistPerAxis(target, PlayerCamera.up) / smoothness * -Mathf.Sign(moveInputDelta.x);
        }

        private bool IsPlayerMovingTowardsTarget(AimAssistTarget target, float xInputDelta)
        {
            return Mathf.RoundToInt(Mathf.Sign(PlayerCamera.InverseTransformPoint(target.transform.position).x)) == Mathf.RoundToInt(Mathf.Sign(xInputDelta));
        }

        private float CalculateAssistPerAxis(AimAssistTarget target, Vector3 axis)
        {
            var playerToTarget = target.transform.position - PlayerCamera.position;
            var playerToTargetPerpendicular = Quaternion.Euler(axis * 90) * playerToTarget;
            var cosVpPTp = Vector3.Dot(playerPhysics.Velocity, playerToTargetPerpendicular) / (playerPhysics.Velocity.magnitude * playerToTargetPerpendicular.magnitude);
            return Mathf.Atan((playerPhysics.Velocity * cosVpPTp).magnitude * Time.deltaTime / playerToTarget.magnitude) * Mathf.Rad2Deg;
        }

        private void SetUpPlayerPhysicsInfo()
        {
            if (playerBody)
            {
                playerPhysics = new RigidbodyInfo(playerBody);
                return;
            }

            if (playerController)
            {
                playerPhysics = new CharacterControllerInfo(playerController);
                return;
            }

            throw new MissingComponentException("Magnetism needs either a Rigidbody or a CharacterController set via Inspector.");
        }
    }
}
