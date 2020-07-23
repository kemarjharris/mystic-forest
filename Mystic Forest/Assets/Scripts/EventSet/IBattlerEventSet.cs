using UnityEngine;
using System.Collections;

public interface IBattlerEventSet
{
    System.Action<IBattler> onBattlerHit { get; set; }
    System.Action<IBattler> onBattlerRecovered { get; set; }
    System.Action onEventExecuted { get; set; }
    System.Action onComboFinished { get; set; }
    System.Action onPlayerSwitchedIn { get; set; }
    System.Action onPlayerSwitchedOut { get; set; }
}
