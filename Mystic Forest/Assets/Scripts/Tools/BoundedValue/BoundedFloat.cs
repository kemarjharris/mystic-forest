using UnityEngine;
using System.Collections;

[System.Serializable]
public class BoundedFloat : BoundedValue<float>
{
    public BoundedFloat(float v, float min, float max) : base(v, min, max) { }
}
