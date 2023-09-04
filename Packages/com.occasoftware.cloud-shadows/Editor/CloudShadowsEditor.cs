using UnityEngine;
using UnityEditor;

namespace OccaSoftware.CloudShadows.Editor
{
    [CustomEditor(typeof(Runtime.CloudShadows))]
    [CanEditMultipleObjects]
    public class CloudShadowsEditor : UnityEditor.Editor
    {
        SerializedProperty cloudLayerRelativeHeight;
        SerializedProperty followTarget;
        SerializedProperty windDirection;
        SerializedProperty windSpeed;

        SerializedProperty fadeInExtents;
        SerializedProperty maximumOpacity;
        SerializedProperty ditherScale;

        SerializedProperty tilingDomain;

        SerializedProperty orientation;
        SerializedProperty cloudiness;
        SerializedProperty cloudTexture;

        SerializedProperty ditherNoiseType;

        void OnEnable()
        {
            cloudLayerRelativeHeight = serializedObject.FindProperty("cloudLayerRelativeHeight");
            followTarget = serializedObject.FindProperty("followTarget");
            windDirection = serializedObject.FindProperty("windDirection");
            windSpeed = serializedObject.FindProperty("windSpeed");

            fadeInExtents = serializedObject.FindProperty("fadeInExtents");
            maximumOpacity = serializedObject.FindProperty("maximumOpacity");
            ditherScale = serializedObject.FindProperty("ditherScale");
            tilingDomain = serializedObject.FindProperty("tilingDomain");
            orientation = serializedObject.FindProperty("orientation");
            cloudiness = serializedObject.FindProperty("cloudiness");
            cloudTexture = serializedObject.FindProperty("cloudTexture");
            ditherNoiseType = serializedObject.FindProperty("ditherNoiseType");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Cloud Look Options", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(cloudiness, new GUIContent("Cloudiness", "Amount of cloudiness in the scene."));
            EditorGUILayout.PropertyField(
                orientation,
                new GUIContent("Orientation", "Orientation (in degrees) of the cloud texture on the XZ plane.")
            );
            EditorGUILayout.PropertyField(cloudTexture, new GUIContent("Texture", "The texture to use for the cloud shape."));
            EditorGUILayout.PropertyField(
                tilingDomain,
                new GUIContent("Tiling Domain", "The XZ grid size over which the texture will tile one time.")
            );
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Cloud Dithering Options", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(fadeInExtents, new GUIContent("Fade in Extents"));
            EditorGUILayout.PropertyField(maximumOpacity, new GUIContent("Maximum Opacity"));
            EditorGUILayout.PropertyField(ditherScale, new GUIContent("Dither Scale"));
            EditorGUILayout.PropertyField(ditherNoiseType, new GUIContent("Dither Noise Type"));
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Cloud Wind Options", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(windSpeed, new GUIContent("Wind Speed", "Rate at which the clouds will move. (Meters per second)."));
            if (windSpeed.floatValue > 0f)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(
                    windDirection,
                    new GUIContent("Wind Direction", "Direction (in degrees) that the overall cloud shape will move in the XZ plane.")
                );
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Follow Target Options", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(
                followTarget,
                new GUIContent("Target", "Optional transform to follow. When set, the clouds will be centered above this object.")
            );
            if (followTarget.objectReferenceValue != null)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(
                    cloudLayerRelativeHeight,
                    new GUIContent("Relative Height", "Maintains a fixed relative Y Offset relative to the target transform.")
                );
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
