using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DirectionCommandPicker<T> : IDirectionCommandPicker where T : IDirectionPickable
{

    private IEnumerable<T> commandables;
    private List<Direction> inputtedDirections;
    private DirectionCommandButton inputtedButton;
    public IUnityTimeService service = new UnityTimeService(); 
    float timeOfLastInput;

    protected DirectionCommandPicker() {
        inputtedDirections = new List<Direction>(); 
        inputtedButton = DirectionCommandButton.NULL;
    }

    public T inputSelect()
    {
        if (existingInput() && service.unscaledTime - timeOfLastInput > 0.2f)
        {
            clear();
        }
        Direction dir = DirectionalInput.GetSimpleDirection();

        if (inputtedDirections.Count <= 0)
        {
            if (dir != Direction.NULL)
            {
                inputtedDirections.Add(dir);
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
                timeOfLastInput = service.unscaledTime;
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
        return default;
    }

    public void set(IEnumerable<T> commandables) {
        this.commandables = commandables;
    }

    public void clear()
    {
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
