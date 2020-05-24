using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetSet : ITargetSet
{
    Transform target;

    public void SetTarget(Transform target) => this.target = target;
    
}
