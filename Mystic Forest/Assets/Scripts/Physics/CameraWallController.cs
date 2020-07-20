using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraWallController : MonoBehaviour
{

    new public Camera camera;
    public GameObject wallPrefab;
    public GameObject leftWall;
    public GameObject rightWall;

    // Use this for initialization
    void Start()
    {

        if (leftWall == null)
        {
            leftWall = Instantiate(wallPrefab);
            leftWall.name = "Left Camera Wall";
        }
        if (rightWall == null)
        {
            rightWall = Instantiate(wallPrefab);
            rightWall.name = "Right Camera Wall";
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {

        // Calculate the planes from the main camera's view frustum
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        TransformWall(leftWall.transform, new Vector3(0, 0.5f, 10), planes[0].normal);
        TransformWall(rightWall.transform, new Vector3(1,0.5f,10), planes[1].normal);

    }

    void TransformWall(Transform wallTransform, Vector3 viewportPoint, Vector3 planeNormal)
    {
        Vector3 pos = camera.ViewportToWorldPoint(viewportPoint);
        Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, planeNormal);
        wallTransform.position = pos;
        wallTransform.rotation = rotation;
    }

}
