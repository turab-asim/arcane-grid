using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Target;
using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.TargetSelection;
using UnityEngine;

namespace Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.AimAssists
{
    /// <summary>
    /// Base class for all aim assist classes that handles the setup of the target selector.
    /// </summary>
    [RequireComponent(typeof(TargetSelector))]
    public abstract class AimAssistBase : MonoBehaviour
    {
        /// <summary>
        /// The current target available from the selector
        /// </summary>
        public AimAssistTarget Target => targetSelector.Target;
        
        /// <summary>
        /// The radius of the selector in metres
        /// </summary>
        public float AimAssistRadius => targetSelector.aimAssistRadius;
        
        /// <summary>
        /// The near clip distance of the selector
        /// </summary>
        public float NearClipDistance => targetSelector.nearClipDistance;
        
        /// <summary>
        /// The far clip distance of the selector
        /// </summary>
        public float FarClipDistance => targetSelector.farClipDistance;

        /// <summary>
        /// The player camera that is used for aiming.
        /// </summary>
        public Transform PlayerCamera => targetSelector.playerCamera;

        public NotifyTargetFound OnTargetFound => targetSelector.OnTargetSelected;

        public NotifyTargetFound OnTargetLost => targetSelector.OnTargetLost;      
        
        [Header("Master switch")] [Tooltip("Enable aim assist")]
        public bool aimAssistEnabled = true;
        
        private TargetSelector targetSelector;
        
        protected virtual void Awake()
        {
            SetUpTargetSelector();
            CheckPlayerCamera();
        }

        private void SetUpTargetSelector()
        {
            targetSelector = GetComponent<TargetSelector>();
        }

        private void CheckPlayerCamera()
        {
            if (PlayerCamera)
            {
                return;
            }

            throw new MissingComponentException("Player camera transform is missing for Aim Assist Script.");
        }
    }
}
