using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Helper.Numerics;
using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Model;
using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Target;
using UnityEngine;

namespace Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.AimAssists
{
    /// <summary>
    /// Smoothly rotates the player towards the target. A curve is available to smooth out the rotation and allow some wiggle room.
    /// </summary>
    public class AimLock : AimAssistBase
    {
        private const float AngleThresholdBelowTarget = 15f;
        private const float UnderAimMultiplier = 0.85f;

        [Header("Timings")] [Tooltip("How much time it should take to get from the edge of the aim assist to the center of the target, on horizontal axis.")]
        public float horizontalTimeToAim = 2;

        [Tooltip("How much time it should take to get from the edge of the aim assist to the center of the target, on vertical axis.")]
        public float verticalTimeToAim = 1;
        
        [Header("Smooth aimlock")]
        [Tooltip("Enables or disables the angular velocity dampening curve.")]
        public bool enableAngularVelocityCurve = true;
        
        [Tooltip("Angular velocity curve to multiply the aim assist with. Values closer to 0 refer to the crosshair being close to the target, e.g. looking at its center.")]
        public AnimationCurve angularVelocityCurve;

        /// <summary>
        /// Smoothly snaps aim to the target's position, at its center.
        /// 
        /// Takes in the delta time as it can be different based on where the input logic is implemented (Update, fixed update etc) or scaled time.
        /// 
        /// Returns the needed adjustment in degrees for the pitch and rotation.
        /// This adjustment is an addition - you need to add it to your turn / pitch.
        /// </summary>
        /// <returns>the additional rotation in degrees. add this to your rotation and pitch.</returns>
        public AimAssistResult AssistAim()
        {
            if (!aimAssistEnabled)
            {
                return AimAssistResult.Empty;
            }

            var target = Target;

            if (!target || IsTargetBelowPlayer(target))
            {
                return AimAssistResult.Empty;
            }

            var targetPos = target.transform.position;
            var totalHorizontalRotationAngles = CalculateTotalRotationAngles(Vector3.up, targetPos);
            var totalVerticalRotationAngles = CalculateTotalRotationAngles(PlayerCamera.right, targetPos);

            var playerAimToTargetInLocal = CalculatePlayerAimToTargetLocal(targetPos);

            var dx = CalculateDeltaRotationDegrees(totalVerticalRotationAngles, verticalTimeToAim, Time.deltaTime, targetPos);
            var dy = CalculateDeltaRotationDegrees(totalHorizontalRotationAngles, horizontalTimeToAim, Time.deltaTime, targetPos);

            if (enableAngularVelocityCurve)
            {
                dx *= SampleCurveForVertical(playerAimToTargetInLocal);
                dy *= SampleCurveForHorizontal(playerAimToTargetInLocal);
            }

            return new AimAssistResult(pitchAdditionInDegrees: dx.Sanitized(), rotationAdditionInDegrees: dy.Sanitized(),
                turnAddition: dy.Sanitized() * Vector3.up);
        }

        private Vector3 CalculatePlayerAimToTargetLocal(Vector3 target)
        {
            var playerToTarget = target - PlayerCamera.position;
            var playerForwardInTargetDistance = PlayerCamera.forward * playerToTarget.magnitude;
            var forwardEndToTarget = playerToTarget - playerForwardInTargetDistance;
            return PlayerCamera.InverseTransformVector(forwardEndToTarget);
        }

        private bool IsTargetBelowPlayer(AimAssistTarget target)
        {
            return Mathf.Acos(Vector3.Dot(Vector3.down, (target.transform.position - PlayerCamera.position).normalized)) * Mathf.Rad2Deg < AngleThresholdBelowTarget;
        }

        private float CalculateDeltaRotationDegrees(float totalRotation, float timeToAim, float deltaTime, Vector3 target)
        {
            var adjustedTimeToAim = timeToAim * AimAssistRadius;
            var distance = (target - PlayerCamera.transform.position).magnitude;
            var angularVelocity = Mathf.Atan2(1f, distance) * Mathf.Rad2Deg / adjustedTimeToAim;
            return Mathf.Min(angularVelocity * deltaTime, Mathf.Abs(totalRotation) * UnderAimMultiplier) * Mathf.Sign(totalRotation);
        }

        private float SampleCurveForHorizontal(Vector3 playerAimToTargetLocal)
        {
            var sample = Mathf.Abs(playerAimToTargetLocal.x) / AimAssistRadius;
            return angularVelocityCurve.Evaluate(sample);
        }

        private float SampleCurveForVertical(Vector3 playerAimToTargetLocal)
        {
            var sample = Mathf.Abs(playerAimToTargetLocal.y) / AimAssistRadius;
            return angularVelocityCurve.Evaluate(sample);
        }

        private float CalculateTotalRotationAngles(Vector3 planeNormal, Vector3 target)
        {
            var camForwardProjected = Vector3.ProjectOnPlane(PlayerCamera.forward, planeNormal);
            var playerToTargetProjected = Vector3.ProjectOnPlane((target - PlayerCamera.position).normalized, planeNormal);
            return Vector3.SignedAngle(camForwardProjected, playerToTargetProjected, planeNormal);
        }
    }
}
