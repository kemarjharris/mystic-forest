using UnityEngine;
using System.Collections;
using System;

public class HitPoint : MonoBehaviour, IHitBox
{
    public void CheckCollision(Action<Collider2D> onCollide)
    {
        if (onCollide == null) return;
        Collider2D[] overlapColliders = Physics2D.OverlapPointAll(transform.position);
        for (int i = 0; i < overlapColliders.Length; i++)
        {
             onCollide?.Invoke(overlapColliders[i]);
        }
    }

}
