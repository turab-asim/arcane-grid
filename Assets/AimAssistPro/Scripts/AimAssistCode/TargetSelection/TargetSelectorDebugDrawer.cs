using UnityEngine;

namespace Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.TargetSelection
{
    /// <summary>
    /// Debug drawer that shows what the target selector sees in the Scene view.
    /// </summary>
    [RequireComponent(typeof(TargetSelector))]
    [ExecuteInEditMode]
    public class TargetSelectorDebugDrawer : MonoBehaviour
    {
#if UNITY_EDITOR
        private TargetSelector targetSelector;

        private void OnEnable()
        {
            targetSelector = GetComponent<TargetSelector>();
        }
        
        private void OnDrawGizmos()
        {
            if (!targetSelector || !targetSelector.playerCamera)
            {
                return;
            }
                        
            Gizmos.color = Color.green;
            var startCenter = targetSelector.playerCamera.position + targetSelector.playerCamera.forward * targetSelector.nearClipDistance;
            var endCenter = targetSelector.playerCamera.position + targetSelector.playerCamera.forward * targetSelector.farClipDistance;
            Gizmos.DrawWireSphere(startCenter, targetSelector.aimAssistRadius);
            Gizmos.DrawWireSphere(endCenter, targetSelector.aimAssistRadius);
            Gizmos.DrawLine(startCenter - targetSelector.playerCamera.right * targetSelector.aimAssistRadius, endCenter - targetSelector.playerCamera.right * targetSelector.aimAssistRadius);
            Gizmos.DrawLine(startCenter + targetSelector.playerCamera.right * targetSelector.aimAssistRadius, endCenter + targetSelector.playerCamera.right * targetSelector.aimAssistRadius);
            Gizmos.DrawLine(startCenter - targetSelector.playerCamera.up * targetSelector.aimAssistRadius, endCenter - targetSelector.playerCamera.up * targetSelector.aimAssistRadius);
            Gizmos.DrawLine(startCenter + targetSelector.playerCamera.up * targetSelector.aimAssistRadius, endCenter + targetSelector.playerCamera.up * targetSelector.aimAssistRadius);
        }
#endif
    }
}