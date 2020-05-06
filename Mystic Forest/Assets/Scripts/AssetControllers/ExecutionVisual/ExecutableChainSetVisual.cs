using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ExecutableChainSetVisual
{

    GameObject parent;
    readonly List<ExecutionVisual> visuals = new List<ExecutionVisual>();

    public ExecutableChainSetVisual(IEnumerable<IExecutableChain> chains)
    {
        GameObject arrowPrefab = Resources.Load<GameObject>("Prefabs/ExecutionVisual/Arrow");
        parent = new GameObject("Executable Chain Set Visual");
        GameObject canvas = GameObject.Find("Canvas");
        parent.transform.SetParent(canvas.transform);
        float width = arrowPrefab.GetComponent<RectTransform>().rect.width * arrowPrefab.transform.localScale.x;
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
                arrow.transform.position = new Vector3(i * width, 3 * j);

            }
            new ExecutableChainVisual(chain, new Vector3(i * width * 1.1f, 3 * j), parent.transform);
            // ExecutionVisualFactory.CreateVisual(chain.head, attacker.transform.position + new Vector3(i * width * 1.1f, 3 * j), parent.transform); 
            //AttackVisual visual = Object.Instantiate(Resources.Load<AttackVisual>("Prefabs/ExecutableAttackVisual"), parent.transform);
            //visual.transform.position = attacker.transform.position + new Vector3(i * width * 1.1f, 3 * j);
            j++;
        }
        parent.transform.localScale = new Vector2(0.7f, 0.7f);
    }
}