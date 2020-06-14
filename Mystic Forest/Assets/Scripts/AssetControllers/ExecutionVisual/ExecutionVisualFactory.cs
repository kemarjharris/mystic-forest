using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExecutionVisualFactory
{
    public static ExecutionVisual CreateVisual(IExecutable executable, Vector3 position, Transform parent)
    {
        string button = executable.GetButton() == DirectionCommandButton.J_OR_K ? "J/K" : executable.GetButton().ToString();
        if (executable is OnReleaseHoldExecutable)
        {
            HoldVisual visualPrefab = Resources.Load<HoldVisual>("Prefabs/ExecutionVisual/ExecutableHoldVisual");
            HoldVisual visual = Object.Instantiate(visualPrefab, position, Quaternion.identity, parent.transform);
            visual.Initialize((OnReleaseHoldExecutable)executable);
            visual.SetText(button);
            return visual;
        }
        else if (executable is KeyDownMashExecutable)
        {
            ExpandingButtonMashVisual visualPrefab = Resources.Load<ExpandingButtonMashVisual>("Prefabs/ExecutionVisual/ExecutableMashVisual");
            ExpandingButtonMashVisual visual = Object.Instantiate(visualPrefab, position, Quaternion.identity, parent.transform);
            visual.Initialize((KeyDownMashExecutable) executable);
            visual.SetText(button);
            return visual;
        } else
        {
            ExecutionVisual visualPrefab = Resources.Load<ExecutionVisual>("Prefabs/ExecutionVisual/ExecutableVisual");
            ExecutionVisual visual = Object.Instantiate(visualPrefab, position, Quaternion.identity, parent.transform);
            visual.SetText(button);
            return visual;
        }
    }
}
