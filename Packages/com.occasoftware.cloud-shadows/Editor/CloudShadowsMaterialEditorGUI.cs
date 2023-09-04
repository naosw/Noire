using UnityEngine;
using UnityEditor;
using System;

namespace OccaSoftware.CloudShadows.Editor
{
    public class CloudShadowsMaterialEditorGUI : ShaderGUI
    {
        public override void OnGUI(MaterialEditor e, MaterialProperty[] properties)
        {
            EditorGUILayout.LabelField("Configure all cloud shadows properties from the CloudShadows script");
        }
    }
}
