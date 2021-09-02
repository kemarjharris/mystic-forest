using UnityEngine;
using System.Collections;

public static class TransformTools
{
    public static void SetGlobalScale(this Transform transform, Vector3 globalScale)
    {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
    }

    public static Vector3 YRotate(this Transform transform, Vector3 vectorToRotate)
    {
        return Quaternion.Euler(0, transform.eulerAngles.y, 0) * vectorToRotate;
    }
}
