using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetSet : ITargetSet
{
    Transform target;
    bool floorPoint;


    public Transform GetTarget() => target;

    public bool IsFloorPoint() => floorPoint;
    public bool SetFloorPoint(bool floorPoint) => this.floorPoint = floorPoint;
    public void SetTarget(Transform target) => this.target = target;

}
