using UnityEngine;
using UnityEditor;

public interface IPlayableAnim
{
    float GetSpeed();
    AnimationClip GetAnimationClip();
    Vector2 Evaluate(float time);
    string GetName();
}