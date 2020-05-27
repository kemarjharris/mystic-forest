using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
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

    public static VectorZ zero { get => new VectorZ(0, 0); }

    public static Vector3 operator +(VectorZ a, Vector3 b) => new Vector3(a.x, a.y, a.z) + b;
    public static Vector3 operator +(Vector3 a, VectorZ b) => a + new Vector3(b.x, b.y, b.z);
    public static VectorZ operator +(VectorZ a, VectorZ b) => new VectorZ(a.x + b.x, a.z + b.z);
    public static VectorZ operator *(VectorZ a, float b) => new VectorZ(a.x * b, a.z * b);
    public static bool operator ==(VectorZ a, VectorZ b) => a.x == b.x && a.y == b.y && a.z == b.z;
    public static bool operator !=(VectorZ a, VectorZ b) => !(a == b);
    public override bool Equals(object obj) => new Vector3(x, y, z).Equals(obj);
    public static implicit operator Vector3(VectorZ v) => new Vector3(v.x, v.y, v.z);
    public override int GetHashCode() => new Vector3(x, y, z).GetHashCode();

}
