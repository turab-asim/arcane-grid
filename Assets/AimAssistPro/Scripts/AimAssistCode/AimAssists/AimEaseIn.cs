using UnityEngine;

namespace Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.AimAssists
{
    /// <summary>
    /// Picks a more dominant axis for look input (horizontal or vertical) and downscales the other axis by a given factor.
    ///
    /// This makes it easier to do horizontal or vertical turns on the controller.
    /// </summary>
    public class AimEaseIn : MonoBehaviour
    {
        [Header("Master switch")] [Tooltip("Enables or disables the aim assist.")]
        public bool aimAssistEnabled = true;
        
        [Tooltip("Determines the multiplier for the less dominant axis for the input. The lower the value, the less the less dominant axis will be taken into account.")]
        [Range(0.1f, 1f)]
        public float smoothnessMultiplier = 0.6f;

        /// <summary>
        /// Downscales the less dominant axis in input.
        ///
        /// The input is the controller input delta.
        /// The output is the modified controller input delta - NOT the actual angles to rotate.
        ///
        /// Run your look input through this before proceeding with the rotations.
        /// </summary>
        /// <param name="lookInputDelta">controller look input delta</param>
        /// <returns>the modified look input delta</returns>
        public Vector2 AssistAim(Vector2 lookInputDelta)
        {
            if (!aimAssistEnabled)
            {
                return lookInputDelta;
            }
            
            var xMagnitude = Mathf.Abs(lookInputDelta.x);
            var yMagnitude = Mathf.Abs(lookInputDelta.y);
            var x = xMagnitude < yMagnitude ? lookInputDelta.x * smoothnessMultiplier : lookInputDelta.x;
            var y = yMagnitude < xMagnitude ? lookInputDelta.y * smoothnessMultiplier : lookInputDelta.y;
            return new Vector2(x, y);
        }
    }
}
