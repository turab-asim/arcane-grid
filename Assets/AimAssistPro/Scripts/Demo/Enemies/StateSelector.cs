using UnityEngine;

namespace Assets.Agoston_R.Aim_Assist_Pro.Scripts.Demo.Enemies
{
    class StateSelector
    {
        public bool TargetAquired
        {
            set
            {
                targetAquired = value;
                selectedState = EvaluateTargetState();
            }
        }
        public bool TargetActivated 
        { 
            set 
            { 
                targetActivated = value;
                selectedState = EvaluateTargetState();
            }
        }

        public DemoTargetState SelectedState => selectedState;

        private DemoTargetState selectedState = DemoTargetState.Lost;
        private bool targetAquired;
        private bool targetActivated;

        private DemoTargetState EvaluateTargetState()
        {
            if (!targetAquired)
            {
                return DemoTargetState.Lost;
            }

            if (targetActivated)
            {
                return DemoTargetState.Activated;
            }

            return DemoTargetState.Aquired;
        }
    }
}
