using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Target;

namespace Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Helper.Caching
{
    /// <summary>
    /// Contains logic that handles the selection of a single target and calling its notification events.
    ///
    /// Must use distinct instances for each player, in case multiple players are present.
    /// </summary>
    public class SelectedTargetStore
    {
        private AimAssistTarget selectedTarget;

        /// <summary>
        /// Calls the events to the selected target and stores it to prevent invoking the same events over and over again.
        /// </summary>
        /// <param name="target">target that was found</param>
        public void ProcessTarget(AimAssistTarget target)
        {
            if (target)
            {
                OnTargetFound(target);
            }
            else
            {
                NotifyAndEraseTargetIfExists();
            }
        }
        
        private void NotifyAndStoreTarget(AimAssistTarget target)
        {
            selectedTarget = target;
            selectedTarget.TargetSelected.Invoke();
        }
        
        private void NotifyAndEraseTargetIfExists()
        {
            if (!selectedTarget)
            {
                return;
            }
            selectedTarget.TargetLost.Invoke();
            selectedTarget = null;
        }
        
        private void OnTargetFound(AimAssistTarget target)
        {
            if (target == selectedTarget)
            {
                return;
            }
            NotifyAndEraseTargetIfExists();
            NotifyAndStoreTarget(target);
        }
    }
}