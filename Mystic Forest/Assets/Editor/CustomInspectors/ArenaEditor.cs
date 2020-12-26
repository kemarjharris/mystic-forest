using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RectangleArenaSetUp))]
public class ArenaEditor : Editor
{

    Vector2 size;
    float height;

    private void Awake()
    {
        RectangleArenaSetUp arena = target as RectangleArenaSetUp;
        Vector3 groundSize = Vector3.Scale(arena.ground.collider.size, arena.transform.localScale);
        size = new Vector2(groundSize.x, groundSize.z);
        height = arena.walls[0].collider.size.y;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        size  = EditorGUILayout.Vector2Field("Ground Size", size);
        height = EditorGUILayout.FloatField("Wall Height", height);
        height = Mathf.Max(height, 0.1f);
        RectangleArenaSetUp arena = target as RectangleArenaSetUp;
        arena.UpdateArenaSize(new Vector3(size.x, height, size.y));

    }
}
