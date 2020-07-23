using UnityEngine;
using System.Collections;
using System;

public struct BattlerEventSet : IBattlerEventSet
{
    public Action<IBattler> onBattlerHit { get; set; }
    public Action<IBattler> onBattlerRecovered { get; set; }
    public Action onEventExecuted { get; set; }
    public Action onComboFinished { get; set; }
}
