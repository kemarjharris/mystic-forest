using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Executable/Execution Event/Animation Event")]
public class AnimationEvent : ExecutionEvent
{
    public PlayableAnimSO animation;

    public override void OnExecute(IBattler attacker, ITargetSet targets)
    {
        attacker.Play(animation);
    }
}