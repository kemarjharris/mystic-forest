using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MeleeEvent : ExecutionEvent
{
    float timeOfContact;
    PlayableAnimSO animSO;

    public ICollection<IPlayableAnim> GetAnims() => new IPlayableAnim[] { animSO };

    public override void OnExecute(IBattler attacker, ITargetSet targets)
    {
        
    }
}