using UnityEngine;
using System.Collections;

public class SpriteXRotation : MonoBehaviour
{

    Transform cameraTransform;


    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.rotation =
            Quaternion.Euler(
                transform.eulerAngles.y >= Mathf.Abs(180) ? -cameraTransform.eulerAngles.x : cameraTransform.eulerAngles.x,
                transform.eulerAngles.y,
                transform.eulerAngles.z);
    }
}
