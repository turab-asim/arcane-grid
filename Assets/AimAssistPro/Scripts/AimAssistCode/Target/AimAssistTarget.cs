using System;
using UnityEngine;
using UnityEngine.Events;

namespace Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Target
{
    /// <summary>
    /// GameObjects with this component can be targeted by the Aim Assist component to adjust the aim.
    /// </summary>
    public class AimAssistTarget : MonoBehaviour, IEquatable<AimAssistTarget>
    {
        /// <summary>
        /// Can be invoked when the target is picked up by the aim assist.
        ///
        /// Invoked once when the target is picked up but is not repeatedly invoked while the aim assist lasts.
        /// </summary>
        [Header("Events")]
        public readonly UnityEvent TargetSelected = new UnityEvent();

        /// <summary>
        /// Invoked when the target is no longer picked up by the aim assist.
        /// </summary>
        public readonly UnityEvent TargetLost = new UnityEvent();

        private void Awake()
        {
            CheckForCollider();
        }

        public bool Equals(AimAssistTarget other)
        {
            return other != null && GetInstanceID() == other.GetInstanceID();
        }

        private void CheckForCollider()
        {
            if (!GetComponent<Collider>())
            {
                Debug.LogWarning($"No collider found on target {name}, the aim assist won't take effect.");
            }
        }


    }
}