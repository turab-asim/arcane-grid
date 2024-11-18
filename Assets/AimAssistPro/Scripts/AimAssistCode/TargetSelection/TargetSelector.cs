using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Helper.Caching;
using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Target;
using UnityEngine;

namespace Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.TargetSelection
{
    /// <summary>
    /// Finds and selects a given target for the aim assists and invokes events on the target if any are defined.
    ///
    /// Separating the selection can enable you to use multiple aim assists together with no additional performance hit from the target selection process.
    /// </summary>
    public class TargetSelector : MonoBehaviour
    {
        [Header("Data for aim assist")] [Tooltip("The player's camera that will be aim assisted")]
        public Transform playerCamera;

        [Tooltip("The radius of the aim assist in metres.")]
        public float aimAssistRadius = 0.5f;

        [Tooltip("The near clip distance in metres. Aim assist doesn't work for targets closer than this.")]
        public float nearClipDistance = 0.5f;

        [Tooltip("The far clip distance in metres. Aim assist doesn't work for target further than this. Increasing this takes more computing power.")]
        public float farClipDistance = 50f;

        [Header("Layers")]
        [Tooltip("Layers to take into account during the aim assist.")]
        public LayerMask layerMask;

        public NotifyTargetFound OnTargetSelected { get; } = new NotifyTargetFound();
        public NotifyTargetFound OnTargetLost { get; } = new NotifyTargetFound();

        /// <summary>
        /// The target that is currently found by the selector. Null if currently no targets are found.
        /// </summary>
        public AimAssistTarget Target { get; private set; }
        
        private readonly Cache<AimAssistTarget> targetCache = Cache<AimAssistTarget>.Instance;
        private readonly SelectedTargetStore selectedTargetStore = new SelectedTargetStore();
        
        private void Start()
        {
            CheckPlayerCamera();
            targetCache.Purge();
        }

        private void FixedUpdate()
        {
            var foundTarget = SelectClosestTarget();

            if (foundTarget != null)
            {
                NotifyOnTargetFound(foundTarget);
            } 
            else
            {
                NotifyOnTargetLost();
            }

            Target = foundTarget;
        }

        private void NotifyOnTargetFound(AimAssistTarget foundTarget)
        {
            if (foundTarget != Target)
            {
                OnTargetSelected?.Invoke(foundTarget);
            }
        }

        private void NotifyOnTargetLost()
        {
            if (Target != null)
            {
                OnTargetLost?.Invoke(Target);
            }
        }

        private AimAssistTarget SelectClosestTarget()
        {
            var target = SelectTarget();
            selectedTargetStore.ProcessTarget(target);
            return target;
        }

        private AimAssistTarget SelectTarget()
        {
            var direction = playerCamera.transform.forward;
            var startPoint = playerCamera.position + playerCamera.forward * nearClipDistance;

            if (!Physics.SphereCast(startPoint, aimAssistRadius, direction, out var hit, farClipDistance, layerMask))
            {
                return null;
            }

            var target = targetCache.FindOrInsert(hit.collider);

            if (target)
            {
                return target;
            }

            if (!Physics.Raycast(startPoint, direction, out var raycastHit, farClipDistance, layerMask))
            {
                return null;
            }

            return targetCache.FindOrInsert(raycastHit.collider);
        }

        private void CheckPlayerCamera()
        {
            if (playerCamera)
            {
                return;
            }

            throw new MissingComponentException("Player camera transform is missing for Aim Assist Script.");
        }
    }
}