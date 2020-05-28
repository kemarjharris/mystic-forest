using UnityEngine;
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
        BoxColliderDrawer.DrawBoxCollider(transform, Color.red, center, extents * 2);
    }
}
