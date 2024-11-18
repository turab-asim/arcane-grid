using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Target;
using Assets.Agoston_R.Aim_Assist_Pro.Scripts.Demo.Enemies;
using System.Collections;
using UnityEngine;

namespace Agoston_R.Aim_Assist_Pro.Scripts.Demo.Enemies
{
    /// <summary>
    /// Changes the color when hit by player fire.
    /// </summary>
    public class DemoTarget : AimAssistTarget
    {
        [Header("Color")]
        [Tooltip("The color to change to when the target is hit by player fire.")]
        public Color activatedColor;

        [Tooltip("The color to change to when the target is aquired but the player isn't shooting at it.")]
        public Color aquiredColor;

        private Color originalColor;
        private Renderer rendererComponent;
        private IEnumerator changeActivationStateRoutine;

        private StateSelector stateSelector = new StateSelector();
        
        private void Awake()
        {
            SetUpMeshRenderer();
            SaveOriginalColor();
        }

        private void Start()
        {
            SubscribeEvents();
        }

        private void OnDestroy()
        {
            TearDownEvents();
        }

        /// <summary>
        /// Changes the target color when hit, resets the color after a timer is complete.
        /// </summary>
        public void ActivateTarget()
        {
            if (changeActivationStateRoutine != null)
            {
                StopCoroutine(changeActivationStateRoutine);
            }
            changeActivationStateRoutine = ActivateThenReset();
            StartCoroutine(changeActivationStateRoutine);
        }

        private void OnTargetAquired()
        {
            stateSelector.TargetAquired = true;
            RecolorTargetByState(stateSelector.SelectedState);
        }

        private void OnTargetLost()
        {
            stateSelector.TargetAquired = false;
            RecolorTargetByState(stateSelector.SelectedState);
        }
        
        private void SaveOriginalColor()
        {
            originalColor = rendererComponent.material.color;
        }

        private IEnumerator ActivateThenReset()
        {
            stateSelector.TargetActivated = true;
            RecolorTargetByState(stateSelector.SelectedState);

            yield return new WaitForSeconds(0.1f);

            stateSelector.TargetActivated = false;
            RecolorTargetByState(stateSelector.SelectedState);
        }

        private void TearDownEvents()
        {
            TargetSelected.RemoveAllListeners();
        }

        private void SubscribeEvents()
        {
            TargetSelected.AddListener(OnTargetAquired);
            TargetLost.AddListener(OnTargetLost);
        }

        private void RecolorTargetByState(DemoTargetState state)
        {
            var mat = rendererComponent.material;
            switch (state)
            {
                case DemoTargetState.Aquired:
                    mat.color = aquiredColor;
                    break;
                case DemoTargetState.Activated:
                    mat.color = activatedColor;
                    break;
                case DemoTargetState.Lost:
                    mat.color = originalColor;
                    break;
            }        
        }

        private void SetUpMeshRenderer()
        {
            rendererComponent = GetComponent<Renderer>();

            if (rendererComponent)
            {
                return;
            }

            throw new MissingComponentException($"Renderer missing from Demo Target {name}");
        }
    }
}