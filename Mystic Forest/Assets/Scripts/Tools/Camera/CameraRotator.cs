using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraRotator : MonoBehaviour
{

    [Range(-360, 360)] public float rotation;
    [Range(-720, 720)]public float y;
    public Vector3 rotationPoint;
    public Vector3 rotationOffset;

    // Update is called once per frame
    void Update()
    {
        float yRotation = rotation * Mathf.Deg2Rad;
        float zOffset = Mathf.Cos(yRotation) * rotationOffset.z;
        float xOffset = Mathf.Sin(yRotation) * rotationOffset.z;

        transform.position = rotationPoint + new Vector3(xOffset, rotationOffset.y , zOffset);
        transform.rotation= Quaternion.Euler(transform.eulerAngles.x, rotation, transform.eulerAngles.z);

        /*

        float angle = y % 360;
        if (angle < 0)
        {
            angle += 360;
        }
        */
        

        // Debug.Log(Mathf.DeltaAngle(0, y));

    }



}
