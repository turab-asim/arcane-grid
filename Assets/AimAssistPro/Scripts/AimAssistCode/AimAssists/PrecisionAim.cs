using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Target;
using UnityEngine;

namespace Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.AimAssists
{
    /// <summary>
    /// Slows down the look input using a curve to ease up aim on the target.
    /// </summary>
    public class PrecisionAim : AimAssistBase
    {
        private const float AngleThresholdBelowTarget = 15f;

        [Header("Sensitivity")]
        [Tooltip("The sensitivity multiplier at the center of the aim assist. This will be lerped from the outer edge of the radius.")]
        [Range(0.001f, 0.99f)]
        public float sensitivityMultiplierAtCenter = 0.18f;

        [Tooltip("The sensitivity multiplier at the edge of the aim assist. This will be eased out back to the original sensitivity when the assist loses the target. " +
            "Has to be more than center multiplier (or will be set to center multiplier).")]
        [Range(0.1f, 0.99f)]
        public float sensitivityMultiplierAtEdge = 0.5f;

        [Header("Ease Out")]
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
        /// Calculates the slowed down player input delta using the curve.
        ///
        /// Receives a look input delta, returns a modified look input delta.
        ///
        /// Before calculating your rotations from the player input, run that input through this.
        /// </summary>
        /// <param name="lookInputDelta">inputs: the player's look input delta</param>
        /// <returns>the modified look input delta</returns>
        public Vector2 AssistAim(Vector2 lookInputDelta)
        {
            var target = Target;
            if (!aimAssistEnabled)
            {
                return lookInputDelta;
            }

            if (sensitivityMultiplierAtEdge < sensitivityMultiplierAtCenter)
            {
                sensitivityMultiplierAtEdge = sensitivityMultiplierAtCenter;
            }

            if (target == null)
            {
                return LerpEaseOut(lookInputDelta);
            }

            if (TargetBelowPlayer(target))
            {
                return lookInputDelta;
            }

            var playerAimToTargetLocal = CalculatePlayerAimToTargetLocal(target.transform.position);
            var a = SampleCurveForVertical(playerAimToTargetLocal);
            var b = SampleCurveForHorizontal(playerAimToTargetLocal);

            return new Vector2(b * lookInputDelta.x, a * lookInputDelta.y);
        }

        private bool TargetBelowPlayer(AimAssistTarget target)
        {
            return Mathf.Acos(Vector3.Dot(Vector3.down, (target.transform.position - PlayerCamera.position).normalized)) * Mathf.Rad2Deg < AngleThresholdBelowTarget;
        }

        private Vector3 CalculatePlayerAimToTargetLocal(Vector3 target)
        {
            var playerToTarget = target - PlayerCamera.position;
            var playerForwardInTargetDistance = PlayerCamera.forward * playerToTarget.magnitude;
            var forwardEndToTarget = playerToTarget - playerForwardInTargetDistance;
            return PlayerCamera.InverseTransformVector(forwardEndToTarget);
        }

        private float SampleCurveForHorizontal(Vector3 playerAimToTargetLocal)
        {
            var factor = Mathf.Abs(playerAimToTargetLocal.x) / AimAssistRadius;
            return CalculatePlayerAimToTargetMultiplier(factor);
        }

        private float SampleCurveForVertical(Vector3 playerAimToTargetLocal)
        {
            var factor = Mathf.Abs(playerAimToTargetLocal.y) / AimAssistRadius;
            return CalculatePlayerAimToTargetMultiplier(factor);
        }

        private float CalculatePlayerAimToTargetMultiplier(float factor)
        {
            return Mathf.Lerp(sensitivityMultiplierAtCenter, 1, factor / AimAssistRadius);
        }

        private Vector2 LerpEaseOut(Vector2 lookInputDelta)
        {
            timeAccumulator = Mathf.Min(timeAccumulator + Time.deltaTime, timeToRegainOriginalInputSensitivity);
            return Mathf.Lerp(sensitivityMultiplierAtEdge, 1, timeAccumulator / timeToRegainOriginalInputSensitivity) * lookInputDelta;
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
