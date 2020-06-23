using UnityEngine;
using System.Collections;

public interface IComboCounter
{
    System.Action<int> onCountIncremented { get; set; }
    System.Action onComboFinished { get; set; }

    void OnBattlerHit(IBattler battler);

    void OnBattlerRecovered(IBattler battler);
}
