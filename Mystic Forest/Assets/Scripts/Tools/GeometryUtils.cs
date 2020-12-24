using UnityEngine;
using System.Collections;

public class GeometryUtils
{
    public static Quaternion PlaneRotation(Vector3 a, Vector3 b, Vector3 c) => Quaternion.FromToRotation(Vector3.forward, -(new Plane(a,b,c)).normal);
}
