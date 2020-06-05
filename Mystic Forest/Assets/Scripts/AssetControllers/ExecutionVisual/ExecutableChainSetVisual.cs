using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ExecutableChainSetVisual : MonoBehaviour
{

    IExecutionModule module;
    ExecutableChainVisual chainVisual;
    private GameObject parent;
    readonly List<ExecutionVisual> visuals = new List<ExecutionVisual>();

    private void Awake()
    {
        module = GameObject.FindGameObjectWithTag("Execution Module").GetComponent<IExecutionModule>();
       // if (module == null) module = new GameObject("Execution Module").AddComponent<ExecutionModule>(); 
    }

    private void OnEnable()
    {
        module.OnNewSetLoaded.AddAction(OnStart);
        module.OnNewChainLoaded.AddAction(OnNewChainLoaded);
        module.OnChainFired.AddAction(OnStart);
        module.OnChainFinished.AddAction(OnChainFinished);
    }

    private void OnDisable()
    {
        module.OnNewSetLoaded.RemoveAction(OnStart);
        module.OnNewChainLoaded.RemoveAction(OnNewChainLoaded);
        module.OnChainFired.RemoveAction(OnStart);
        module.OnChainFinished.RemoveAction(OnChainFinished);
    }

    void OnNewChainLoaded(ICustomizableEnumerator<IExecutable> chain)
    {
        if (parent != null) Destroy(parent);
        chainVisual = new ExecutableChainVisual(chain);
        chain.SetOnMoveNext(chainVisual.MoveNext);

    }

    void OnChainFinished()
    {
        if (parent != null) Destroy(parent);

        if (chainVisual != null)
            chainVisual.Destroy();
    }

    void OnStart()
    {
        if (parent != null) Destroy(parent);
        CreateNewSetVisual(module.set);
        parent.transform.localPosition = new Vector2(100, -129);
        parent.transform.localRotation = Quaternion.identity;
    }

    

    private void CreateNewSetVisual(IEnumerable<IExecutableChain> chains)
    {
        GameObject arrowPrefab = Resources.Load<GameObject>("Prefabs/ExecutionVisual/Arrow");
        parent = new GameObject("Executable Chain Set Visual");
        GameObject canvas = GameObject.Find("Canvas");
        parent.transform.SetParent(canvas.transform);
        float width = arrowPrefab.GetComponent<RectTransform>().rect.width * arrowPrefab.transform.localScale.x * 20;
        int j = 1;
        foreach (IExecutableChain chain in chains)
        {
            IDirectionCommand command = chain.GetDirectionCommand();

            int i = 0;
            for (; i < command.directions.Length; i++)
            {
                float angle = 0;
                switch (command.directions[i])
                {
                    case Direction.N:
                        angle = 90;
                        break;
                    case Direction.W:
                        angle = 180;
                        break;
                    case Direction.S:
                        angle = 270;
                        break;
                }
                GameObject arrow = Object.Instantiate(arrowPrefab, parent.transform);
                arrow.transform.Rotate(new Vector3(0, 0, angle));
                arrow.transform.position = new Vector3(i * width, 60 * j);
                arrow.transform.localScale *= 20;

            }
            new ExecutableChainVisual(chain.GetEnumerator(), new Vector3(i * width, 60 * (j-1)), parent.transform).parent.transform.localScale *= 20;
            j++;
        }
        parent.transform.localScale = new Vector2(0.7f, 0.7f);
    }
}