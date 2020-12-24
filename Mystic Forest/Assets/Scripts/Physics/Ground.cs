using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Ground : MonoBehaviour
{
    public new BoxCollider collider { get; private set; }

    private void Awake()
    {
        collider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        // move collider down so the top of the collider represents the ground
        transform.position = new Vector3(0, -ColliderHeight(), 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        collider.center = Vector3.zero;
    }

    public Vector3 FloorPosition()
    {
        return transform.position + (Vector3.up * ColliderHeight());
    }

    

    private float ColliderHeight()
    {
        // get the height of the collider 
        collider.size = new Vector3(collider.size.x, Mathf.Max(collider.size.y, 5), collider.size.z);
        float heightOfCollider = collider.size.y * transform.localScale.y / 2;
        return heightOfCollider;
    }

    public void OnDrawGizmosSelected()
    {
        BoxColliderDrawer.DrawBoxCollider(transform, Color.black, collider.center, collider.size, transform.rotation, 0.2f);
    }
}
