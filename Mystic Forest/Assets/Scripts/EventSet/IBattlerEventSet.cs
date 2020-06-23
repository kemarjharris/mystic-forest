using UnityEngine;
using System.Collections;

public interface IBattlerEventSet
{
    System.Action<IBattler> onBattlerHit { get; set; }
    System.Action<IBattler> onBattlerRecovered { get; set; }

}
