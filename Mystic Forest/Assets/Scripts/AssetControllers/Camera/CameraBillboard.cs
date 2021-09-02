using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class CameraBillboard : MonoBehaviour
{
    static Plane[] planes;
    static float frame;
    static Quaternion quaternion;
    public Transform main;
    public bool flipped;

    private void Start()
    {
        planes = new Plane[6];
        frame = Time.frameCount;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (frame != Time.frameCount)
        {
            // calculate plane camera is on
            planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

            // calculate y rotation of plane
            Quaternion quaternion = Quaternion.FromToRotation(Vector3.forward, -planes[5].normal);
            frame = Time.frameCount;
        }



        float xRotation = Camera.main.transform.eulerAngles.x;
        float yRotation = Camera.main.transform.eulerAngles.y;
        float localYRotation = transform.localEulerAngles.y;
        float thisYRotation = (transform.eulerAngles.y - transform.localRotation.eulerAngles.y) % 360;

        flipped = ShouldFlip(yRotation, thisYRotation);
        
        if (flipped)
        {
            xRotation *= -1;
            yRotation += 180;
            // GetComponent<SpriteRenderer>().color = Color.red;


            
        } else
        {
            // GetComponent<SpriteRenderer>().color = Color.blue;
        }
        
        quaternion.eulerAngles = new Vector3(xRotation, yRotation, transform.eulerAngles.z);
        transform.rotation = quaternion;


        /*
        Debug.Log(main.right);
        Vector3 v = xRotation < 0 ? Quaternion.Euler(0, -45, 0) * transform.right : transform.right;
        Debug.Log(Vector3.SignedAngle(main.right, v, Vector3.up));
        */
        // Transform cameraTransform = Camera.main.transform;

        // https://answers.unity.com/questions/52656/how-i-can-create-an-sprite-that-always-look-at-the.html
        // Comment by Sarrixx
        // Vector3 cameraFwd = new Vector3(cameraTransform.forward.x, transform.forward.y, cameraTransform.transform.forward.z);
        // transform.forward = cameraFwd;

       // transform.rotation =
         //   Quaternion.Euler(
           //     transform.eulerAngles.y >= Mathf.Abs(180) ? -cameraTransform.eulerAngles.x : cameraTransform.eulerAngles.x,
             //   transform.eulerAngles.y % 180,
               // transform.eulerAngles.z);
    }


    float PositiveAngle(float angle)
    {
        angle = angle % 360;
        if (angle < 0)
        {
            angle += 360;
        }
        return angle;
    }

    bool ShouldFlip(float angle, float angle2)
    {
        angle2 = PositiveAngle(angle2);
        if (angle >= 90 && angle <= 270) // middle
        {
            return angle2 < angle - 90 || angle2 > angle + 90;
        } else if (angle < 90)
        {
            // critical angle will be negative
            float criticalAngle = PositiveAngle(angle - 90);
            // eg angle is 45, critical angle will be 315
            // angle needs to be greater than 135 but less than 315 to be flipped


            return (angle2 > angle + 90 && angle2 < criticalAngle);
        } else // angle > 270
        {
            // eg angle is 300
            float criticalAngle = PositiveAngle(angle + 90);
            // ang has to be less than 210
            // angle alse has to be greater than 30 
            return (angle2 < angle - 90 && angle2 > criticalAngle);   
        }
    }

}

/*


// midpoint between player and target
Vector3 midpoint = new Vector3((transform.position.x + target.transform.position.x) / 2, 0, (transform.position.z + target.position.z) / 2);
// rotate only along y axis
Quaternion rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, GeometryUtils.PlaneRotation(transform.position.position, midpoint).eulerAngles.y, transform.rotation.eulerAngles.z);

transform.rotation = rotation;*/