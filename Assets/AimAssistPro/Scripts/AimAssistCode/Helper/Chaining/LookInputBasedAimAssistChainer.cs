using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.AimAssists;
using UnityEngine;

namespace Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Helper.Chaining
{
    /// <summary>
    /// Helper class to integrate multiple types of aim assists that work based on changing the look input.
    /// These aim assists are the AimEaseIn, PrecisionAim and AutoAim. 
    /// 
    /// Look input based aim assists change your look input and you make use of them by turning your player / camera based on that changed look input. 
    /// However when integrating multiple of these, you'd have to run your look input through multiple aim assists and use the end result. 
    /// 
    /// This class helps by chaining the raw look input through the aim assists, and returning the end result. 
    /// You can just use that end result as your look input after running them through this class.
    /// </summary>
    public sealed class LookInputBasedAimAssistChainer
    {
        private Vector2 lookInputDelta;
        private PrecisionAim precisionAim;
        private AimEaseIn aimEaseIn;
        private AutoAim autoAim;

        public LookInputBasedAimAssistChainer WithLookInputDelta(Vector2 lookInputDelta)
        {
            this.lookInputDelta = lookInputDelta;
            return this;
        }

        public LookInputBasedAimAssistChainer UsingPrecisionAim(PrecisionAim precisionAim)
        {
            this.precisionAim = precisionAim;
            return this;
        }

        public LookInputBasedAimAssistChainer UsingAimEaseIn(AimEaseIn aimEaseIn)
        {
            this.aimEaseIn = aimEaseIn;
            return this;
        }

        public LookInputBasedAimAssistChainer UsingAutoAim(AutoAim autoAim)
        {
            this.autoAim = autoAim;
            return this;
        }

        /// <summary>
        /// Combines the results of look input based aim assists into a single end result, using the aim assists provided.
        /// 
        /// If the method is called without using any aim assists, it will just return the original look input delta without changing it.
        /// </summary>
        /// <returns>The modified look input delta that went through all the aim assists that were included.</returns>
        public Vector2 GetModifiedLookInputDelta()
        {
            if (autoAim != null)
            {
                lookInputDelta = autoAim.AssistAim(lookInputDelta);
            }

            if (aimEaseIn != null)
            {
                lookInputDelta = aimEaseIn.AssistAim(lookInputDelta);
            }

            if (precisionAim != null)
            {
                lookInputDelta = precisionAim.AssistAim(lookInputDelta);
            }

            return lookInputDelta;
        }
    }
}
