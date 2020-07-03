using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(ExecutionModule))]
public class TextExecutableChainSetVisual : MonoBehaviour
{
    public Text text;
    IExecutionModule module;
    IExecutableChainSet set;


    private void Awake()
    {
        module = GetComponent<IExecutionModule>();
        module.OnNewSetLoaded.AddAction(SetChainSet);
        text.text = "";
    }

    private void OnDestroy()
    {
        module.OnNewSetLoaded.RemoveAction(SetChainSet);
    }

    public void SetChainSet()
    {
        set = module.set;
    }

    // Update is called once per frame
    void Update()
    {
        if (set == null) return;
        string moduleText = "";
        foreach (IExecutableChain chain in set)
        {
            moduleText += chain.GetDirectionCommand().ToString() + ": " + chain + "\n";

        }
        text.text = moduleText;
    }
}
