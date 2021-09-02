using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MagicMeter))]
public class MagicMeterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Fill"))
        {
            MagicMeter m = target as MagicMeter;
            m.Value = m.settings.maxMana;
        }
    }
}
