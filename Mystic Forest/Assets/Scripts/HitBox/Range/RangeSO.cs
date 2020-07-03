using UnityEngine;
using System.Collections;

public abstract class RangeSO : ScriptableObject
{
    public abstract bool BattlerInRange(Transform battlerTranform);
}
