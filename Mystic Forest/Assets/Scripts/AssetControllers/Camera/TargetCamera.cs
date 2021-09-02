using UnityEngine;
using System.Collections;
using Zenject;

public class TargetCamera : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 0, -10);
    PlayerSwitcher switcher;
    Vector3 velocity = Vector3.zero;
    Quaternion derivative = Quaternion.identity;
    Vector3 rotateVelocity;
    public float smoothTime = 0.1f;

    IPlayer main;
    //Transform target;

    Vector3 smoothDampDestinationVector;
    Quaternion smoothDampDestinationQuaternion;

    Transform curr;
    IRoutine smoothDamp;
    

    

    [Inject]
    public void Construct(PlayerSwitcher switcher)
    {
        this.switcher = switcher;
    }

    public void Awake()
    {
        switcher.onPlayerSwitched += SwapFollowTransform;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        VectorRotation vr;
        if (main.target == null)
        {
            Vector3 right = Quaternion.Euler(0, main.transform.rotation.eulerAngles.y, 0) * Vector3.right;
            vr = RotateAndPositionCamera(main.transform.position-right,main.transform.position+ right);
            
        } else
        {
             vr = RotateAndPositionCamera(main.transform.position, main.target.transform.position);
        }

        // start camera movement when target changes
        if (curr != main.target)
        {
            if (smoothDamp != null) smoothDamp.Stop();
            smoothDamp = new RoutineImpl(SmoothDampCamera(vr), this);
            smoothDamp.OnRoutineFinished += () => smoothDamp = null;
            curr = main.target;
            smoothDamp.Start();
        }
        else if (smoothDamp == null)
        {
            transform.position = vr.v;
            transform.rotation = vr.r;
        }   
    }

    public void OnDestroy()
    {
        switcher.onPlayerSwitched -= SwapFollowTransform;
    }

    public void SwapFollowTransform(IPlayer player)
    {
        main = player;
    }

    VectorRotation RotateAndPositionCamera(Vector3 a, Vector3 b)
    {
        // midpoint between player and target
        Vector3 midpoint = new Vector3((a.x + b.x) / 2, 0, (a.z + b.z) / 2);

        Quaternion rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
            GeometryUtils.PlaneRotation(a, b, midpoint).eulerAngles.y,
            transform.rotation.eulerAngles.z);

        float yRotation = posAngle(rotation.eulerAngles.y); 

        if (Quaternion.Angle(rotation, transform.rotation) > 90) {
            Vector3 temp = a;
            a = b;
            b = temp;
            yRotation = posAngle(yRotation + 180);
        }

        float yRotationRad = yRotation * Mathf.Deg2Rad;
        float zOffset = Mathf.Cos(yRotationRad) * offset.z;
        float xOffset = Mathf.Sin(yRotationRad) * offset.z;

        Vector3 followTransformPosition =  new Vector3((a.x + b.x) / 2, transform.position.y, (a.z + b.z) / 2) + new Vector3(xOffset, 0, zOffset);
        Quaternion followRotation = Quaternion.Euler(rotation.eulerAngles.x, yRotation, rotation.eulerAngles.z);
        return new VectorRotation(followTransformPosition, followRotation);


    }

    private void OnDrawGizmos()
    {
        if (main != null && main.target != null)
        {
            Gizmos.DrawLine(main.transform.position, main.target.transform.position);
        } else if (main != null)
        {
            Vector3 right = Quaternion.Euler(0, main.transform.rotation.eulerAngles.y, 0) * Vector3.right;
            Gizmos.DrawLine(-right, right);
        }
        
    }

    IEnumerator SmoothDampCamera(VectorRotation vr)
    {

        for (float i = 0; i <= smoothTime + 0.1f; i += Time.deltaTime)
        {
            // https://forum.unity.com/threads/quaternion-smoothdamp.793533/
            transform.position = Vector3.SmoothDamp(transform.position, vr.v, ref velocity, smoothTime);
            transform.rotation = QuaternionUtil.SmoothDamp(transform.rotation, vr.r, ref derivative, smoothTime);
            yield return null;
        }
    }

     float mod(float x, float d)
    {
        float r = x % d;
        return r < 0 ? r + d : r;
    }

    float posAngle(float angle) => (angle < 0 ? angle + 360 : angle) % 360;

    private struct VectorRotation
    {
        public Vector3 v;
        public Quaternion r;
        public VectorRotation(Vector3 v, Quaternion r)
        {
            this.v = v;
            this.r = r;
        }
    }

}