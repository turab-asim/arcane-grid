using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Helper.Caching;
using Agoston_R.Aim_Assist_Pro.Scripts.Demo.Enemies;
using UnityEngine;

namespace Agoston_R.Aim_Assist_Pro.Scripts.Demo
{
    /// <summary>
    /// Shoot a raycast that will find a target.
    /// </summary>
    public class Shooter : MonoBehaviour
    {
        private const float MaxShootDistance = 50f;
        
        public Transform cameraOrigin;
        public bool Trigger { get; set; }
        
        [Tooltip("Layers to take into account when shooting.")]
        public LayerMask layerMask;

        private readonly Cache<DemoTarget> targetCache = Cache<DemoTarget>.Instance;
        
        private void Awake()
        {
            CheckCameraOrigin();
        }

        private void FixedUpdate()
        {
            if (!Trigger)
            {
                return;
            }

            if (!ShootRayForTarget(out var target))
            {
                return;
            }

            target.ActivateTarget();
        }

        private bool ShootRayForTarget(out DemoTarget target)
        {
            target = null;
            if (!Physics.Raycast(cameraOrigin.position, cameraOrigin.forward, out var hit, MaxShootDistance, layerMask, QueryTriggerInteraction.Collide))
            {
                return false;
            }

            target = targetCache.FindOrInsert(hit.collider);
            return target != null;
        }
        
        private void CheckCameraOrigin()
        {
            if (!cameraOrigin)
            {
                throw new MissingComponentException("Camera origin not set for shooter script.");
            }
        }
    }
}