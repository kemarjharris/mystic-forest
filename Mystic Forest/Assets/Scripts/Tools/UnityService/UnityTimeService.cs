using UnityEngine;
using UnityEditor;

public interface IUnityTimeService
{
    float unscaledTime { get; }
    float unscaledDeltaTime { get; }
    int frameCount { get; }
}

public class UnityTimeService : IUnityTimeService
{
    public float unscaledTime => Time.unscaledTime;
    public float unscaledDeltaTime => Time.unscaledDeltaTime;
    public int frameCount => Time.frameCount;
}
