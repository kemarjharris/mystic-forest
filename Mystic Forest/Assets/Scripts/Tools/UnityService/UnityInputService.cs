using UnityEngine;
public interface IUnityInputService
{
    bool GetKeyDown(string input);
    bool GetKeyUp(string input);
    bool GetKey(string input);
    bool GetKeyDown(KeyCode key);
    bool GetKeyUp(KeyCode key);
    bool GetKey(KeyCode key);
}

public class UnityInputService : IUnityInputService
{
    public bool GetKeyDown(string input) => Input.GetKeyDown(input);
    public bool GetKeyUp(string input) => Input.GetKeyUp(input);
    public bool GetKey(string input) => Input.GetKey(input);

    public bool GetKeyDown(KeyCode key) => Input.GetKeyDown(key);
    public bool GetKeyUp(KeyCode key) => Input.GetKeyUp(key);
    public bool GetKey(KeyCode key) => Input.GetKey(key);
}