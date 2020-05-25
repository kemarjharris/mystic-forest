using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct VectorZ 
{

    public float x;
    public float y { get => z; }
    public float z;

    public VectorZ(float x, float yz)
    {
        this.x = x;
        z = yz;
    }

    public Vector3 ToVector3() => new Vector3(x, y, z);

    public static Vector3 operator +(VectorZ a, Vector3 b) => a.ToVector3() + b;
    public static Vector3 operator +(Vector3 a, VectorZ b) => a + b.ToVector3();
    public static VectorZ operator +(VectorZ a, VectorZ b) => new VectorZ(a.x + b.x, a.z + b.z);
    public static Vector3 operator *(VectorZ a, float b) => a.ToVector3() * b;

}
