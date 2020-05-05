using UnityEngine;
using UnityEditor;

public interface IUnityInputService
{
    bool GetKeyDown(string input);
}

public class UnityInputService : IUnityInputService {
    public bool GetKeyDown(string input) => Input.GetButtonDown(input);
}