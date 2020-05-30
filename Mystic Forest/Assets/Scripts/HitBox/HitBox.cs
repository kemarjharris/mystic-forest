using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class HitBox : MonoBehaviour, IHitBox
{
    BoxCollider hitCollider;
    
    private void Awake()
    {
        hitCollider = GetComponent<BoxCollider>();
    }

    public void CheckCollision(Action<Collider> onCollide)
    {
        if (onCollide == null) return;
        //Debug.Break();
       // Vector3 scale = Vector3.Scale(hitCollider., transform.localScale);
        Collider[] overlapColliders = Physics.OverlapBox(transform.position + hitCollider.center, hitCollider.size / 2, transform.rotation);
        for (int i = 0; i < overlapColliders.Length; i ++)
        {
            if (hitCollider != overlapColliders[i])
            {
                onCollide?.Invoke(overlapColliders[i]);
            }
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        DrawBoxCollider(Color.red, boxCollider);
    }

    private void DrawBoxCollider(Color gizmoColor, BoxCollider boxCollider, float alphaForInsides = 0.3f)
    {
        //Save the color in a temporary variable to not overwrite changes in the inspector (if the sent-in color is a serialized variable).
        var color = gizmoColor;

        //Change the gizmo matrix to the relative space of the boxCollider.
        //This makes offsets with rotation work
        //Source: https://forum.unity.com/threads/gizmo-rotation.4817/#post-3242447
        Gizmos.matrix = Matrix4x4.TRS(this.transform.TransformPoint(boxCollider.center), this.transform.rotation, this.transform.lossyScale);

        //Draws the edges of the BoxCollider
        //Center is Vector3.zero, since we've transformed the calculation space in the previous step.
        Gizmos.color = color;
        Gizmos.DrawWireCube(Vector3.zero, boxCollider.size);

        //Draws the sides/insides of the BoxCollider, with a tint to the original color.
        color.a *= alphaForInsides;
        Gizmos.color = color;
        Gizmos.DrawCube(Vector3.zero, boxCollider.size);
    }
}
