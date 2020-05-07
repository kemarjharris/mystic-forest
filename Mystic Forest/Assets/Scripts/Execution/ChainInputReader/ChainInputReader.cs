using UnityEngine;
using UnityEditor;

public class ChainInputReader : IChainInputReader
{
    public IUnityInputService service = new UnityInputService();
    public string ReadInput()
    {
        foreach (DirectionCommandButton button in EnumUtil.values<DirectionCommandButton>())
        {
            string key = button.ToString().ToLower();
            if (service.GetKey(key) || service.GetKeyUp(key)) return key;
        }
        return "";
    }
}