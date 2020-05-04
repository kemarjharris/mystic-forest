using UnityEngine;
using UnityEditor;

public interface IUnityInputService
{
    float GetAxisRaw(string axisName);
}

public class UnityInputService : IUnityInputService
{
    public float GetAxisRaw(string axisName) => Input.GetAxisRaw(axisName);
}