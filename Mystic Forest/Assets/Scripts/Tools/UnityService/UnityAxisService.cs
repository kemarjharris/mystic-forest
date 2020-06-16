using UnityEngine;
using UnityEditor;

public interface IUnityAxisService
{
    float GetAxisRaw(string axisName);
    float GetAxis(string axisName);
    float GetAxisDown(string axisName);
}

public class UnityAxisService : IUnityAxisService
{
    public float GetAxisRaw(string axisName) => Input.GetAxisRaw(axisName);
    public float GetAxis(string axisName) => Input.GetAxis(axisName);
    public float GetAxisDown(string axisName) {
        if (axisName == "Horziontal")
        {
            return DirectionalInputDown.InputOnAxisDown(true);
        } else if (axisName == "Vertical")
        {
            return DirectionalInputDown.InputOnAxisDown(false);
        }
        return 0;
    }
    
}