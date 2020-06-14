using UnityEngine;
using UnityEditor;

public class ChainInputReader : IChainInputReader
{
    public IUnityInputService service = new UnityInputService();
    public string ReadInput()
    {
        if (pressed("k"))
        {
            return "k";
        }
        else if (pressed("j"))
        {
            return "j";
        }
        else return "";
    }

    bool pressed(string key) => service.GetKey(key) || service.GetKeyUp(key);
}