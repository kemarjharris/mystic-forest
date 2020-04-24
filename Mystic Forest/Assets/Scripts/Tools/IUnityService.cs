using UnityEngine;
using System.Collections;

public interface IUnityService
{
    bool GetKeyDown(string input);
    bool GetKeyUp(string input);
}

public class UnityService : IUnityService
{
    public bool GetKeyDown(string input) => Input.GetKeyDown(input);
    public bool GetKeyUp(string input) => Input.GetKeyUp(input);
}
