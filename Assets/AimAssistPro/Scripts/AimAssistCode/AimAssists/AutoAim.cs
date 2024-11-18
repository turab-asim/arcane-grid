using UnityEngine;
using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Target;

namespace Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.AimAssists
{
    /// <summary>
    /// Adjusts the player's aim so that the look input is changed to looking at the target. 
    /// 
    /// Eases the aim out afterwards to make flickshots easier. 
    /// </summary>
    public class AutoAim : AimAssistBase
    {
        [Header("Ease In")]
        [Tooltip("The radius of the center of the target in metres, where the adjustment is not active.")]
        [Min(0.001f)]
        public float deadzoneRadius = 0.01f;

        [Tooltip("The mix factor between player input and the aim adjustment (0 is raw player input, 1 is raw aim adjustment)")]
        [Range(0, 1)]
        public float factor = 0.5f;

        [Tooltip("An angle in degrees to activate the aim assist. If the difference between player input and input needed to aim at the target is larger than this, the aim assist will not interfere, " +
            "because it assumes the players wants to look away from the target. Without this the player would get stuck at aiming at the target.")]
        [Range(1, 120)]
        public float aimAngleThreshold = 120f;

        [Header("Ease Out")]
        [Tooltip("The multiplier that slows down the aim of the player when they look away from the target. " +
            "Helps with overshoot when they look away from the target.")]
        [Range(0.01f, 1f)]
        public float aimEaseOutDampeningMultiplier = 0.6f;

        [Tooltip("The time in seconds to regain the original input sensitivity after leaving the target." +
            "Helps get rid of unnatural, robotic stutter from the aim.")]
        [Min(0.01f)]
        public float timeToRegainOriginalInputSensitivity = 0.5f;

        private float timeAccumulator;

        protected override void Awake()
        {
            base.Awake();
            SubscribeToTargetSelectorEvents();
            SetUpTimeAccumulator();
        }

        private void OnDestroy()
        {
            TearDownTargetSelectorEvents();
        }

        /// <summary>
        /// Calculate the desired look input after the aim assist.
        /// 
        /// Use the resulting look input delta to rotate your camera as if you'd normally do without the aim assist.
        /// </summary>
        /// <param name="lookInputDelta">inputs for the aim assist: the player's look input delta.</param>
        /// <returns>the assisted player look input delta.</returns>
        public Vector2 AssistAim(Vector2 lookInputDelta)
        {
            var target = Target;
            if (!aimAssistEnabled)
            {
                return lookInputDelta;
            }

            if (target == null)
            {
                return LerpEaseOut(lookInputDelta);
            }

            if (AimIsInDeadZone(target))
            {
                return lookInputDelta;
            }

            var targetPos = target.transform.position;
            var horizontalAnglesToTarget = CalculateTotalRotationAngles(Vector3.up, targetPos);
            var verticalAnglesToTarget = CalculateTotalRotationAngles(PlayerCamera.right, targetPos);
            var aimInputToTarget = new Vector2(horizontalAnglesToTarget, verticalAnglesToTarget).normalized * lookInputDelta.magnitude;

            if (InputAndCalculatedAimAngleDifference(aimInputToTarget, lookInputDelta) > aimAngleThreshold)
            {
                return lookInputDelta * aimEaseOutDampeningMultiplier;
            }

            return Vector2.Lerp(lookInputDelta, aimInputToTarget, factor);
        }

        private Vector2 LerpEaseOut(Vector2 lookInputDelta)
        {
            timeAccumulator = Mathf.Min(timeAccumulator + Time.deltaTime, timeToRegainOriginalInputSensitivity);
            return Mathf.Lerp(aimEaseOutDampeningMultiplier, 1, timeAccumulator / timeToRegainOriginalInputSensitivity) * lookInputDelta;
        }

        private float InputAndCalculatedAimAngleDifference(Vector2 aimInputToTarget, Vector2 lookInputDelta)
        {
            return Mathf.Abs(Vector2.SignedAngle(aimInputToTarget, lookInputDelta));
        }

        private bool AimIsInDeadZone(AimAssistTarget target)
        {
            var playerToTarget = target.transform.position - PlayerCamera.position;
            var playerLookPoint = PlayerCamera.forward * playerToTarget.magnitude;
            return (playerToTarget - playerLookPoint).sqrMagnitude < deadzoneRadius * deadzoneRadius;
        }

        private float CalculateTotalRotationAngles(Vector3 planeNormal, Vector3 target)
        {
            var camForwardProjected = Vector3.ProjectOnPlane(PlayerCamera.forward, planeNormal);
            var playerToTargetProjected = Vector3.ProjectOnPlane((target - PlayerCamera.position).normalized, planeNormal);
            return Vector3.SignedAngle(camForwardProjected, playerToTargetProjected, planeNormal);
        }

        private void SubscribeToTargetSelectorEvents()
        {
            OnTargetLost.AddListener(ResetEaseOut);
        }

        private void SetUpTimeAccumulator()
        {
            timeAccumulator = timeToRegainOriginalInputSensitivity;
        }

        private void ResetEaseOut(AimAssistTarget target)
        {
            timeAccumulator = 0f;
        }

        private void TearDownTargetSelectorEvents()
        {
            if (OnTargetLost != null)
            {
                OnTargetLost.RemoveListener(ResetEaseOut);
            }
        }
    }
}
