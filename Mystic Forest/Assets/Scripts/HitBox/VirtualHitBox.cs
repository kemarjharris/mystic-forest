using UnityEngine;
using System.Collections;

public class VirtualHitBox : MonoBehaviour, IHitBox
{

    public Vector3 center;
    public Vector3 extents;
    public Vector3 rotation;

    Quaternion Rotation => Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z) * Quaternion.Euler(rotation);

    public void CheckCollision(System.Action<Collider> onCollide)
    {
        if (onCollide == null) return;
        Collider[] overlapColliders = Physics.OverlapBox(transform.position + (Rotation * center), Vector3.Scale(extents, transform.localScale), Rotation);
        for (int i = 0; i < overlapColliders.Length; i++)
        {
            onCollide?.Invoke(overlapColliders[i]);
        }
    }

    private void OnDrawGizmosSelected()
    {
        BoxColliderDrawer.DrawBoxCollider(transform, Color.red, Rotation * center, extents * 2, Rotation);
    }
}
