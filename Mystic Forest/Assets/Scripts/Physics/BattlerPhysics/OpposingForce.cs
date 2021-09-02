using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class OpposingForce : MonoBehaviour
{

    protected static Dictionary<Collider, OpposingForce> colliderForceMap;

    private void Start()
    {
        if (colliderForceMap == null)
        {
            colliderForceMap = new Dictionary<Collider, OpposingForce>();
        }
        Collider[] colliders = GetComponents<Collider>();
        if (colliders.Length <= 0)
        {
            Debug.LogWarning(string.Format("Gameobject {0} has an opposing force monobehaviour attached, but no gameobject with a collider. It will not register any opposing forces.", gameObject));
        }
        else
        {
            for(int i = 0; i < colliders.Length; i ++)
            {
                colliderForceMap.Add(colliders[i], this);
            }
        }
    }

    private void OnDestroy()
    {
        Collider[] colliders = GetComponents<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliderForceMap.Remove(colliders[i]);
        }

    }

    // how much force, and the direction of the incoming force
    public abstract Vector3 OppositeForce(Vector3 force, Vector3 incomingDirection);

    public static OpposingForce ColliderToOpposingForce(Collider collider)
    {
        return colliderForceMap[collider];
    }
}
