using UnityEngine;
using System.Collections;

public class ExecutablePool : IExecutablePool
{
    public Transform target { get; set; }
    public bool floorPoint { get; set; }
}
