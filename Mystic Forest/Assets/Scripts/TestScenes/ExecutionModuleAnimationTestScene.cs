using System.Collections.Generic;
using UnityEngine;

public class ExecutionModuleAnimationTestScene : MonoBehaviour
{
    public ExecutableChainSetSOImpl set;
    private ExecutionModule module;
    public Battler battler;

    public Dictionary<Vector3, Battler> start = new Dictionary<Vector3, Battler>();

    private void Start()
    {
        module = GameObject.Find("Execution Module").GetComponent<ExecutionModule>();
        Battler[] battlers = FindObjectsOfType<Battler>();
        for (int i = 0; i < battlers.Length; i ++)
        {
            start.Add(battlers[i].transform.position, battlers[i]);
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            foreach (KeyValuePair<Vector3, Battler> pair in start)
            {
                pair.Value.transform.position = pair.Key;
            }
        }
    }
}