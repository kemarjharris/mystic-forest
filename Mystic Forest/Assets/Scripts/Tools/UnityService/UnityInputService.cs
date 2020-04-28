using UnityEngine;
using System.Collections;

public interface IUnityInputService
{
    bool GetKeyDown(string input);
    bool GetKeyUp(string input);
    bool GetKey(string input);
}

public class UnityInputService : IUnityInputService
{
    public bool GetKeyDown(string input) => Input.GetKeyDown(input);
    public bool GetKeyUp(string input) => Input.GetKeyUp(input);
    public bool GetKey(string input) => Input.GetKey(input);

}
