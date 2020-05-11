﻿using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Animation/Playable Animation SO")]
public class PlayableAnimSO : ScriptableObject, IPlayableAnim
{
    public AnimationClip clip;
    public float speed = 1;
    public AnimationCurve xCurve;
    public AnimationCurve yCurve;

    public Vector2 Evaluate(float time) => new Vector2(xCurve.Evaluate(time), yCurve.Evaluate(time));

    public AnimationClip GetAnimationClip() => clip;

    public string GetName() => clip.name;

    public float GetSpeed() => speed;

    public AnimationCurve GetXCurve() => xCurve;

    public AnimationCurve GetYCurve() => yCurve;
}