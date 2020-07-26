using UnityEngine;
using System.Collections;

public interface IBattlerEventSet
{
    // when a battler is hit py an attack
    System.Action<IBattler> onBattlerHit { get; set; }
    // when a battler recovers from hitstun
    System.Action<IBattler> onBattlerRecovered { get; set; }
    // when a battler executes an execution event
    System.Action onEventExecuted { get; set; }
    // when a battler finishes a combo
    System.Action onComboFinished { get; set; }
    // when a battler gets sswitched to
    System.Action onPlayerSwitchedIn { get; set; }
    // when a battler gets switched from
    System.Action onPlayerSwitchedOut { get; set; }
    // when a battle goes into a state in which they are not comboing and not executing
    System.Action onPlayerBecomeInactive { get; set; }
}
