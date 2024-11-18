using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.TargetSelection;
using UnityEngine;

namespace Agoston_R.Aim_Assist_Pro.Scripts.Demo
{
    /// <summmary>
    /// Controls the projector that indicated the edges of the aim assist.
    /// A projector is used instead of a round crosshair to show accurate target selection for the spherecast (that isn't a cone cast).
    /// </summary?
    public class ProjectorController : MonoBehaviour
    {
        private Projector crosshairProjector;

        void Start()
        {
            SetUpProjector();
        }

        private void SetUpProjector()
        {
            crosshairProjector = GetComponent<Projector>();

            if (!crosshairProjector || !FindTargetSelector(out var selector))
            {
                Debug.LogWarning("No projector or target selector found, the crosshair projection is skipped.");
                return;
            }

            crosshairProjector.orthographic = true;
            crosshairProjector.orthographicSize = selector.aimAssistRadius;
        }

        private bool FindTargetSelector(out TargetSelector targetSelector)
        {
            targetSelector = null;
            var player = GameObject.Find("Player");
            if (!player)
            {
                return false;
            }

            targetSelector = player.GetComponent<TargetSelector>();

            return targetSelector != null;
        }
    }
}
