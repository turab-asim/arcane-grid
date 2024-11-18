using UnityEngine;
using System.Collections.Generic;
using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.AimAssists;

namespace Agoston_R.Aim_Assist_Pro.Scripts.Demo.Menu
{
    public class AimAssistSelector : MonoBehaviour
    {
        private const string MagnetismButtonName = "Magnetism";
        private const string AimLockButtonName = "AimLock";
        private const string AimEaseInButtonName = "AimEaseIn";
        private const string AutoAimButtonName = "AutoAim";
        private const string PrecisionAimButtonName = "PrecisionAim";

        private Magnetism magnetism;
        private AimLock aimLock;
        private AimEaseIn aimEaseIn;
        private AutoAim autoAim;
        private PrecisionAim precisionAim;

        private readonly Dictionary<string, AimAssistBase> aimAssistNameToComponent = new Dictionary<string, AimAssistBase>();

        private void Awake()
        {
            SetUpAimAssists();
            StoreNameToAimAssistScripts();
        }

        public void SwitchAimAssist(string buttonName)
        {
            if (aimAssistNameToComponent.TryGetValue(buttonName, out var aimAssist))
            {
                aimAssist.aimAssistEnabled = !aimAssist.aimAssistEnabled;
            }
            else if (buttonName == AimEaseInButtonName)
            {
                aimEaseIn.aimAssistEnabled = !aimEaseIn.aimAssistEnabled;
            }
        }

        public bool GetAimAssistActivationState(string buttonName)
        {
            if (aimAssistNameToComponent.TryGetValue(buttonName, out var aimAssist))
            {
                return aimAssist.aimAssistEnabled;
            }

            return aimEaseIn.aimAssistEnabled;
        }

        private void StoreNameToAimAssistScripts()
        {
            aimAssistNameToComponent.Add(MagnetismButtonName, magnetism);
            aimAssistNameToComponent.Add(AimLockButtonName, aimLock);
            aimAssistNameToComponent.Add(AutoAimButtonName, autoAim);
            aimAssistNameToComponent.Add(PrecisionAimButtonName, precisionAim);
        }

        private void SetUpAimAssists()
        {
            magnetism = GetComponent<Magnetism>();
            aimLock = GetComponent<AimLock>();
            aimEaseIn = GetComponent<AimEaseIn>();
            autoAim = GetComponent<AutoAim>();
            precisionAim = GetComponent<PrecisionAim>();
        }
    }

}
