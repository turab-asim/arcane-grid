using DTT.PublishingTools;
using DTT.Utils.EditorUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.InfiniteScroll.Editor
{
    /// <summary>
    /// The custom editor for the <see cref="InfiniteScroll"/> class.
    /// </summary>
    [CustomEditor(typeof(InfiniteScroll))]
    [DTTHeader("dtt.infinite-scroll")]
    internal class InfiniteScrollEditor : DTTInspector
    {
        /// <summary>
        /// Cache containing all the serialized properties.
        /// </summary>
        private InfiniteScrollSerializedPropertyCache _serializedPropertyCache;
        
        /// <summary>
        /// Cache containing all the GUI content.
        /// </summary>
        private InfiniteScrollGUIContentCache _contentCache;
        
        /// <summary>
        /// Reference to the target of this editor.
        /// </summary>
        private InfiniteScroll _infiniteScroll;
        
        /// <summary>
        /// The toggle used for the inertia.
        /// </summary>
        private AnimatedToggleFoldout _inertiaToggleFoldout;
        
        /// <summary>
        /// The toggle used for the snapping.
        /// </summary>
        private AnimatedToggleFoldout _snappingToggleFoldout;

        /// <summary>
        /// Foldout for the element events.
        /// </summary>
        private AnimatedFoldout _eventsFoldout;
        
        /// <summary>
        /// Initializes all properties and sets up the component correctly for use.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            _inertiaToggleFoldout = new AnimatedToggleFoldout(this);
            _snappingToggleFoldout = new AnimatedToggleFoldout(this);
            _serializedPropertyCache = new InfiniteScrollSerializedPropertyCache(serializedObject);
            _contentCache = new InfiniteScrollGUIContentCache();
            _infiniteScroll = (InfiniteScroll)target;
            _serializedPropertyCache.HorizontalScrollbarProperty.objectReferenceValue = null;
            _serializedPropertyCache.VerticalScrollbarProperty.objectReferenceValue = null;
            _serializedPropertyCache.MovementTypeProperty.enumValueIndex = 
                (int)(_serializedPropertyCache.DisableInfiniteScroll.boolValue
                ? ScrollRect.MovementType.Clamped
                : ScrollRect.MovementType.Unrestricted);
            serializedObject.ApplyModifiedProperties();
            _eventsFoldout = new AnimatedFoldout(this);
        }

        /// <summary>
        /// Draws the inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_serializedPropertyCache.ContentProperty, _contentCache.ContentContent);

            DrawScrollAxes();
            _serializedPropertyCache.UseSnapping.boolValue =
                _snappingToggleFoldout.OnGUI("Use Snapping", _serializedPropertyCache.UseSnapping.boolValue, () =>
                {
                    string name = _serializedPropertyCache.SnapStrength.displayName;
                    string tooltip = _serializedPropertyCache.SnapStrength.tooltip;
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(_serializedPropertyCache.SnapStrength, new GUIContent(name, tooltip));
                    EditorGUI.indentLevel--;
                });
            

            _serializedPropertyCache.InertiaProperty.boolValue =
                _inertiaToggleFoldout.OnGUI("Inertia", _serializedPropertyCache.InertiaProperty.boolValue, () =>
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(_serializedPropertyCache.DecelerationRateProperty, _contentCache.DecelerationRateContent);
                    EditorGUI.indentLevel--;
                });
            
            EditorGUILayout.PropertyField(_serializedPropertyCache.ScrollSensitivity, _contentCache.ScrollSensitivityContent);

            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(_serializedPropertyCache.ViewportProperty, _contentCache.ViewportContent);

            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(_serializedPropertyCache.OnValueChangedProperty, _contentCache.OnValueChangedContent);

            _eventsFoldout.OnGUI("Wrapping Events", () =>
            {
                EditorGUILayout.PropertyField(_serializedPropertyCache.ElementWrappedProperty, _contentCache.ElementWrappedContent);
                EditorGUILayout.PropertyField(_serializedPropertyCache.ElementFullyEnteredViewportProperty, _contentCache.ElementFullyEnteredViewportContent);
                EditorGUILayout.PropertyField(_serializedPropertyCache.ElementFullyExitsViewportProperty, _contentCache.ElementFullyExitsViewportContent);
                EditorGUILayout.PropertyField(_serializedPropertyCache.ElementPartiallyEnteredViewportProperty, _contentCache.ElementPartiallyEnteredViewportContent);
                EditorGUILayout.PropertyField(_serializedPropertyCache.ElementPartiallyExitsViewportProperty, _contentCache.ElementPartiallyExitsViewportContent);
            });

            EditorGUILayout.PropertyField(_serializedPropertyCache.DisableInfiniteScroll);
            
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws the options for the user to be able scroll in.
        /// </summary>
        private void DrawScrollAxes()
        {
            // Retrieve values before input changes.
            bool horizontalOriginal = _serializedPropertyCache.HorizontalProperty.boolValue;
            bool verticalOriginal = _serializedPropertyCache.VerticalProperty.boolValue;

            // Draw toggles.
            bool horizontal = EditorGUILayout.Toggle("Horizontal", horizontalOriginal);
            bool vertical = EditorGUILayout.Toggle("Vertical", verticalOriginal);

            // If both are true we only want to have the most recent one be selected.
            // So we invert the originals to get the inverse.
            if (horizontal && vertical)
            {
                horizontal = !horizontalOriginal;
                vertical = !verticalOriginal;
            }

            // If we have none selected we want to revert to the original.
            // We do this by applying the original values and overriding the input.
            if (!horizontal && !vertical)
            {
                horizontal = horizontalOriginal;
                vertical = verticalOriginal && !horizontal;
            }

            // Then we apply the values to the serialized properties.
            _serializedPropertyCache.HorizontalProperty.boolValue = horizontal;
            _serializedPropertyCache.VerticalProperty.boolValue = vertical;
        }
    }
}