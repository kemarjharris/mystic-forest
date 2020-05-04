using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class DirectionCommandPicker <T> where T : IDirectionPickable {

    protected IEnumerable<T> commandables;
    protected List<Direction> inputtedDirections;
    protected DirectionCommandButton inputtedButton;
    protected Coroutine coroutine;
    protected bool coroutineFinished;
    protected MonoBehaviour runner;

    protected DirectionCommandPicker(MonoBehaviour runner) {
        inputtedDirections = new List<Direction>(); 
        inputtedButton = DirectionCommandButton.NULL;
        this.runner = runner;
    }

    public T inputSelect()
    {
        Direction dir = DirectionalInput.GetSimpleDirection();

        if (inputtedDirections.Count <= 0)
        {
            if (dir != Direction.NULL)
            {
                inputtedDirections.Add(dir);
                coroutine = runner.StartCoroutine(ClearInputAfter(0.5f));
            }
        }
        else // If you've started inputting directions
        {
            // When a new button is pressed
            if (dir != inputtedDirections[inputtedDirections.Count - 1])
            {
                if (inputtedDirections[inputtedDirections.Count - 1] == Direction.NULL)
                {
                    inputtedDirections[inputtedDirections.Count - 1] = dir;
                }
                else
                {
                    inputtedDirections.Add(dir);
                }
                if (coroutine != null)
                {
                    runner.StopCoroutine(coroutine);
                    coroutine = runner.StartCoroutine(ClearInputAfter(0.2f));
                }
            }
        }

        if (Input.GetButtonDown("z"))
        {
            inputtedButton = DirectionCommandButton.Z;
        }
        else if (Input.GetButtonDown("x"))
        {
            inputtedButton = DirectionCommandButton.X;
        }
        else if (Input.GetButtonDown("c"))
        {
            inputtedButton = DirectionCommandButton.C;
        }

        T t = default;

        if (inputtedButton != DirectionCommandButton.NULL)
        {
            // remove placeholder null tacked on the end of the list
            if (inputtedDirections.Count > 0 && inputtedDirections[inputtedDirections.Count - 1] == Direction.NULL)
            {
                inputtedDirections.RemoveAt(inputtedDirections.Count - 1);
            }
            t = Select(new DirectionCommand(inputtedButton, inputtedDirections.ToArray()));

            clear();
            if (t != null)
            {
                return t;
            }
        }
        return default(T);
    }

    public void set(IEnumerable<T> commandables) {
        this.commandables = commandables;
    }

    protected IEnumerator ClearInputAfter(float seconds)
    {
        coroutineFinished = false;
        yield return new WaitForSeconds(seconds);
        coroutineFinished = true;
        // Wait for the next loop around so that the ooption can get chosen
        yield return null;
        inputtedDirections.Clear();
        inputtedButton = DirectionCommandButton.NULL;

    }

    public void clear()
    {
        if (coroutine != null && !coroutineFinished)
        {
            runner.StopCoroutine(coroutine);
            coroutineFinished = true;
        }
        inputtedDirections.Clear();
        inputtedButton = DirectionCommandButton.NULL;

    }

    public bool existingInput()
    {
        return inputtedDirections.Count > 0;
    }

    protected T Select(IDirectionCommand command)
    {
        int hash = command.GetHashCode();
        foreach (IDirectionPickable pickable in commandables)
        {
            if (pickable.GetDirectionCommand().GetHashCode() == hash)
            {
                return (T) pickable;
            }
        }
        return default;
    }

}
