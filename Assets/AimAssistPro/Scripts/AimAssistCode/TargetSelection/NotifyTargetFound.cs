using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Target;
using UnityEngine.Events;

namespace Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.TargetSelection
{
    /// <summary>
    /// A dedicated name for the Unity event that is fired off when a new target is found.
    /// </summary>
    public class NotifyTargetFound : UnityEvent<AimAssistTarget>
    {
    }
}
