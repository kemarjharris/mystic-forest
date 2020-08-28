using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class ViewportPosition : MonoBehaviour
{
    [Range(0, 1)] public float xPos;
    [Range(0, 1)] public float yPos;

    public void LateUpdate()
    {
        transform.position = Camera.main.ViewportToWorldPoint(new Vector3(xPos, yPos, Camera.main.farClipPlane));
        transform.rotation = Camera.main.transform.rotation;
    }
}
