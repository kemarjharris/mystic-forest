using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class ViewportPosition : MonoBehaviour
{
    [Range(0, 1)] public float xPos;
    [Range(0, 1)] public float yPos;
    public bool zOnFarClipPlane = true;

    public void LateUpdate()
    {
        transform.position = Camera.main.ViewportToWorldPoint(new Vector3(xPos, yPos, zOnFarClipPlane ? Camera.main.farClipPlane - 10 : 0));
        transform.rotation = Camera.main.transform.rotation;
    }
}
