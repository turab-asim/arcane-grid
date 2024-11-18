using UnityEngine;

namespace Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Helper.Info
{
    /// <summary>
    /// Physics information on the player.
    ///
    /// Using this as a strategy pattern to eliminate code duplication when working with both rigidbodies and character controllers. 
    /// </summary>
    public interface IPlayerPhysicsInfo
    {
        /// <summary>
        /// The player's velocity
        /// </summary>
        Vector3 Velocity { get; }
    }
}