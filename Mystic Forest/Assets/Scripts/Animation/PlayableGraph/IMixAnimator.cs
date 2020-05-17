using UnityEngine;
using UnityEditor;

public interface IMixAnimator
{
    void Play(IPlayableAnim anim);

    void Stop();
}