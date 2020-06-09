using UnityEngine;
using System.Collections;
using System;

public class HitPoint : MonoBehaviour, IHitBox
{
    public float radius = 0.1f;

    public void CheckCollision(Action<Collider> onCollide)
    {
        if (onCollide == null) return;
        Collider[] overlapColliders = Physics.OverlapSphere(transform.position, radius);
        for (int i = 0; i < overlapColliders.Length; i++)
        {
             onCollide?.Invoke(overlapColliders[i]);
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
