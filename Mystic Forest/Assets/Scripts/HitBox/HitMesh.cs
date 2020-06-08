using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshCollider))]
[ExecuteInEditMode]
public class HitMesh : MonoBehaviour, IHitBox
{
    new MeshCollider collider;
    HashSet<Collider> currentlyColliding;

    public void Awake()
    {
        collider = GetComponent<MeshCollider>();
        collider.convex = true;
        collider.isTrigger = true;
        currentlyColliding = new HashSet<Collider>();
    }

    public void OnTriggerEnter(Collider collider)
    {
        currentlyColliding.Add(collider);
    }

    public void OnTriggerExit(Collider collider)
    {
        currentlyColliding.Remove(collider);
    }

    public void CheckCollision(Action<Collider> onCollide)
    {
        currentlyColliding.RemoveWhere((collider) => collider.gameObject == null);
        foreach (Collider collider in  currentlyColliding)
        {
            onCollide.Invoke(collider);
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireMesh(collider.sharedMesh, transform.position, transform.rotation, transform.lossyScale);
    }
}