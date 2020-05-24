using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetSet : ITargetSet
{
    ISet<Transform> set;

    public TargetSet()
    {
        set = new HashSet<Transform>();
    }

    public void AddTarget(Transform target) => set.Add(target);
    
}
