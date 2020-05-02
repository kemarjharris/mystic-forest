using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExecutionVisualFactory
{
    public static ExecutionVisual CreateVisual(IExecutable executable, Vector3 position, Transform parent)
    {
        if (executable is PressExecutableSO)
        {
            ExecutionVisual visualPrefab = Resources.Load<ExecutionVisual>("Prefabs/ExecutionVisual/ExecutableVisual");
            ExecutionVisual visual = Object.Instantiate(visualPrefab, position, Quaternion.identity, parent.transform);
            return visual;
        } else if (executable is KeyDownMashExecutableSO)
        {
            ExpandingButtonMashVisual visualPrefab = Resources.Load<ExpandingButtonMashVisual>("Prefabs/ExecutionVisual/ExecutableMashVisual");
            ExpandingButtonMashVisual visual = Object.Instantiate(visualPrefab, position, Quaternion.identity, parent.transform);
            return visual;
        } else if (executable is HoldVisual)
        {
            HoldVisual visualPrefab = Resources.Load<HoldVisual>("Prefabs/ExecutionVisual/ExecutableHoldVisual");
            HoldVisual visual = Object.Instantiate(visualPrefab, position, Quaternion.identity, parent.transform);
            return visual;
        } else
        {
            throw new System.ArgumentException("Unhandled Type: " + typeof(IExecutable).DeclaringType.ToString());
        }
    }
}
