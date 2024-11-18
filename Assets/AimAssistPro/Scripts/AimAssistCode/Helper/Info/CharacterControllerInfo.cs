using UnityEngine;

namespace Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Helper.Info
{
    /// <summary>
    /// Physics information from the Character Controller.
    /// </summary>
    public class CharacterControllerInfo : IPlayerPhysicsInfo
    {
        private readonly CharacterController controller;
        
        /// <summary>
        /// The CharacterController's velocity
        /// </summary>
        public Vector3 Velocity => controller.velocity;

        public CharacterControllerInfo(CharacterController controller)
        {
            this.controller = controller;
        }
    }
}