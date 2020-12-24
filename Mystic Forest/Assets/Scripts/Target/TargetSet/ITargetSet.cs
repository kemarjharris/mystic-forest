using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetSet 
{
    void SetTarget(Transform target);

    Transform GetTarget();
}
