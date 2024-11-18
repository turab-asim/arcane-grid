using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.AimAssists;
using Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Helper.Controls;
using UnityEditor;
using UnityEngine;

namespace Agoston_R.Aim_Assist_Pro.Editor
{
    /// <summary>
    /// Custom editor that lets you select character controller or rigidbody to supply velocity for the magnetism.
    /// </summary>
    [CustomEditor(typeof(Magnetism))]
    public class MagnetismVelocityProvider : UnityEditor.Editor
    {
        Magnetism magnetism;

        private void OnEnable()
        {
            magnetism = (Magnetism) target;
        }

        public override void OnInspectorGUI()
        {
            magnetism.controlType = (PlayerControlType) EditorGUILayout.EnumPopup("Player control type", magnetism.controlType);
            switch (magnetism.controlType)
            {
                case PlayerControlType.Rigidbody:
                {
                    magnetism.playerBody = (Rigidbody) EditorGUILayout.ObjectField("Player Body",
                        magnetism.playerBody,
                        typeof(Rigidbody),
                        true);
                    break;
                }
                case PlayerControlType.CharacterController:
                {
                    magnetism.playerController = (CharacterController) EditorGUILayout.ObjectField("Player Controller",
                        magnetism.playerController,
                        typeof(CharacterController),
                        true);
                    break;
                }
            }

            EditorGUILayout.Space();

            base.OnInspectorGUI();
        }
    }
}