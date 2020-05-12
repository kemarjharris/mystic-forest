using UnityEngine;
using UnityEditor;

public interface IUnityTimeService
{
    float unscaledDeltaTime { get; }
}

public class UnityTimeService : IUnityTimeService
{
    public float unscaledDeltaTime => Time.unscaledDeltaTime;
}