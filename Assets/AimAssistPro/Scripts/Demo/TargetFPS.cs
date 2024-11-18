using UnityEngine;

namespace Agoston_R.Aim_Assist_Pro.Scripts.Demo
{
    /// <summary>
    /// Sets the target fps of the scene for demo and playtest purposes.
    /// </summary>
    public class TargetFPS : MonoBehaviour
    {
        [Min(30)]
        public int targetFps = 180;
        
        private void Start()
        {
            Application.targetFrameRate = targetFps;
        }
    }
}