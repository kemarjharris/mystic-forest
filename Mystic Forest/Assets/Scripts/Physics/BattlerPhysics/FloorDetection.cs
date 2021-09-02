using UnityEngine;
using System.Collections;

public class FloorDetection : MonoBehaviour
{

    public float height;
    public float extent;
    public float floorExtent;
    public new BoxCollider collider;
    Vector3 center => -new Vector3(0, ((collider.size.y / 2) - collider.center.y) * collider.transform.localScale.y, 0) + (Vector3.up * height / 2);

    Vector3 right => new Vector3(collider.size.x / 2 * collider.transform.localScale.x + (extent / 2) + 0.01f, 0, 0);
    Vector3 rightSize => new Vector3(extent, height, collider.size.z * collider.transform.localScale.z) / 2;

    Vector3 forward => new Vector3(0, 0, collider.size.z / 2 * collider.transform.localScale.z + (extent / 2) + 0.01f);
    Vector3 forwardSize => new Vector3(collider.size.x * collider.transform.localScale.x, height, extent) / 2;

    Vector3 below => new Vector3(0, -(collider.size.y / 2 * collider.transform.localScale.y) - (floorExtent / 2) + 0.01f, 0);
    Vector3 belowSize => new Vector3(collider.size.x * collider.transform.localScale.x, floorExtent, collider.size.z * collider.transform.localScale.z) / 2;

    Vector3 offset => Vector3.Scale(collider.center, collider.transform.localScale);



    public Collider[][] BoxCast()
    {
        // 0, 1, 2, 3 in N E S W order
        Collider[][] colliders = new Collider[4][];

        // box away from camera
        colliders[0] = Physics.OverlapBox(transform.position + (transform.rotation * (center + forward + offset)), forwardSize, transform.rotation);

        // box to the right
        colliders[1] = Physics.OverlapBox(transform.position + (transform.rotation * (center + right + offset)), rightSize, transform.rotation);

        // box towards camera
        colliders[2] = Physics.OverlapBox(transform.position + (transform.rotation * (center - forward + offset)), forwardSize, transform.rotation);

        // box to the left
        colliders[3] = Physics.OverlapBox(transform.position + (transform.rotation * (center - right + offset)), rightSize, transform.rotation);
        
        

        return colliders;
    }

    public Collider[] FloorCast()
    {
        return Physics.OverlapBox(transform.position + (transform.rotation * (below + offset)), belowSize, transform.rotation);
    }

    private void OnDrawGizmos()
    {

        // box away from camera
        BoxColliderDrawer.DrawBoxCollider(transform, Color.cyan, transform.rotation * (center + forward + offset), forwardSize * 2, transform.rotation);

        // box to the right
        BoxColliderDrawer.DrawBoxCollider(transform, Color.cyan, transform.rotation * (center + right + offset), rightSize * 2, transform.rotation);

        // box towards camera
        BoxColliderDrawer.DrawBoxCollider(transform, Color.cyan, transform.rotation * (center - forward + offset), forwardSize * 2, transform.rotation);

        // box the the left
        BoxColliderDrawer.DrawBoxCollider(transform, Color.cyan, transform.rotation * (center - right + offset), rightSize * 2, transform.rotation);

        // box below
        BoxColliderDrawer.DrawBoxCollider(transform, Color.blue, transform.rotation * (below + offset), belowSize * 2, transform.rotation);
    }
}

