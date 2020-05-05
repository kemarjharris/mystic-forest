using UnityEngine;
using UnityEditor;

public interface IUnityTimeService
{
    float unscaledTime { get; }
}

public class UnityTimeService : IUnityTimeService
{
    public float unscaledTime => Time.unscaledTime;
}

