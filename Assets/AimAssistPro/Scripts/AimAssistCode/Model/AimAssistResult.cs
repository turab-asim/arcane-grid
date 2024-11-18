using UnityEngine;

namespace Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Model
{
    /// <summary>
    /// Contains output from the aim assist calculations.
    ///
    /// The values are unclamped - they are additions. When assigning to properties with a limit like the camera pitch, the resulting value has to be clamped before assigning to the camera pitch.
    /// </summary>
    public struct AimAssistResult
    {
        /// <summary>
        /// The horizontal turn adjustment that's calculated by the aim assist.
        ///
        /// Has to be added to the rotation. Already contains the direction with its Sign.
        ///
        /// When working with quaternions e.g. using MoveRotation for a Rigidbody, make sure you include the original rotation for the RB too
        /// </summary>
        public float RotationAdditionInDegrees { get; }
        
        /// <summary>
        /// The horizontal turn adjustment along the UP axis that's calculated by the aim assist.
        ///
        /// Has to additionally rotate the player by this vector after handling your inputs. Added for convenience over <see cref="RotationAdditionInDegrees"/>
        /// </summary>
        public Vector3 TurnAddition { get; }
        
        /// <summary>
        /// The vertical pitch adjustment that's calculated by the aim assist.
        ///
        /// Has to be added to the pitch of the camera. Already contains the direction with its Sign.
        /// </summary>
        public float PitchAdditionInDegrees { get; }

        public AimAssistResult(float rotationAdditionInDegrees, Vector3 turnAddition, float pitchAdditionInDegrees)
        {
            RotationAdditionInDegrees = rotationAdditionInDegrees;
            TurnAddition = turnAddition;
            PitchAdditionInDegrees = pitchAdditionInDegrees;
        }

        /// <summary>
        /// Returns an empty result. You can add this to your rotations as if they were actual populated values and they'll make no difference.
        /// </summary>
        public static AimAssistResult Empty => new AimAssistResult();
    }
}