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

    public static Vector3 operator +(VectorZ a, Vector3 b) => a + b;
    public static Vector3 operator +(Vector3 a, VectorZ b) => a + new Vector3(b.x, b.y, b.z);
    public static VectorZ operator +(VectorZ a, VectorZ b) => new VectorZ(a.x + b.x, a.z + b.z);
    public static VectorZ operator *(VectorZ a, float b) => new VectorZ(a.x * b, a.z * b);
    public static implicit operator Vector3(VectorZ v) => new Vector3(v.x, v.y, v.z);

}
