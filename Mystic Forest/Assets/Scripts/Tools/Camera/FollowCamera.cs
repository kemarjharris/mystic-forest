using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{

    public Transform followTransform;
    public Vector3 offset = new Vector3(0, 0, -10);

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(followTransform.position.x, transform.position.y, followTransform.position.z) + offset;
    }
}
