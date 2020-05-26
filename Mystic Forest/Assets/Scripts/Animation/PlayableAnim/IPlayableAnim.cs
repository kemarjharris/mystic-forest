using UnityEngine;
using UnityEditor;

public interface IPlayableAnim
{
    float GetSpeed();
    AnimationClip GetAnimationClip();
    Vector3 Evaluate(float time);
    string GetName();
    float GetLength();
}