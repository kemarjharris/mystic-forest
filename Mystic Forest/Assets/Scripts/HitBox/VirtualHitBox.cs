﻿using UnityEngine;
using System.Collections;

public class VirtualHitBox : MonoBehaviour, IHitBox
{

    public Vector3 center;
    public Vector3 extents;

    public void CheckCollision(System.Action<Collider> onCollide)
    {
        if (onCollide == null) return;
        Collider[] overlapColliders = Physics.OverlapBox(transform.position + center, extents, transform.rotation);
        for (int i = 0; i < overlapColliders.Length; i++)
        {
            onCollide?.Invoke(overlapColliders[i]);
        }
    }

    private void OnDrawGizmosSelected()
    {
        DrawBoxCollider(Color.red, center, extents * 2);
    }

    private void DrawBoxCollider(Color gizmoColor, Vector3 center, Vector3 size, float alphaForInsides = 0.3f)
    {
        //Save the color in a temporary variable to not overwrite changes in the inspector (if the sent-in color is a serialized variable).
        var color = gizmoColor;

        //Change the gizmo matrix to the relative space of the boxCollider.
        //This makes offsets with rotation work
        //Source: https://forum.unity.com/threads/gizmo-rotation.4817/#post-3242447
        Gizmos.matrix = Matrix4x4.TRS(transform.TransformPoint(center), transform.rotation, transform.localScale);

        //Draws the edges of the BoxCollider
        //Center is Vector3.zero, since we've transformed the calculation space in the previous step.
        Gizmos.color = color;
        Gizmos.DrawWireCube(Vector3.zero, size);

        //Draws the sides/insides of the BoxCollider, with a tint to the original color.
        color.a *= alphaForInsides;
        Gizmos.color = color;
        Gizmos.DrawCube(Vector3.zero, size);
    }


}
