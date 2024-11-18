using UnityEngine;

namespace Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Helper.Info
{
    /// <summary>
    /// Physics information from the Rigidbody.
    /// </summary>
    public class RigidbodyInfo : IPlayerPhysicsInfo
    {
        private readonly Rigidbody playerBody;
        
        /// <summary>
        /// The player's velocity
        /// </summary>
        public Vector3 Velocity => playerBody.velocity;

        public RigidbodyInfo(Rigidbody playerBody)
        {
            this.playerBody = playerBody;
        }
    }
}