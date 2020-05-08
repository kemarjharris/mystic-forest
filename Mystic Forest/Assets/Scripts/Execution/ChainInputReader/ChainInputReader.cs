using UnityEngine;
using UnityEditor;

public class ChainInputReader : IChainInputReader
{
    public IUnityInputService service = new UnityInputService();
    public string ReadInput()
    {
        string input = "";
        foreach (DirectionCommandButton button in EnumUtil.values<DirectionCommandButton>())
        {
            if (button == DirectionCommandButton.NULL) continue;
            string key = button.ToString().ToLower();
            // += to deal with zx and c all beingpushed at the same time
            if (service.GetKey(key) || service.GetKeyUp(key)) input += key;
        }
        return input;
    }
}