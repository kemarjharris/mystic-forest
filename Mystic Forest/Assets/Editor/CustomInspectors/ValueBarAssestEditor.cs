using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[UnityEditor.CustomEditor(typeof(ValueBarFlash))]
public class ValueBarAssestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Flash"))
        {
            ((ValueBarFlash)target).Flash(); 
        }
    }
}
