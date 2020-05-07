using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DirectionCommandPicker<T> : IDirectionCommandPicker<T> where T : IDirectionPickable
{

    private IEnumerable<T> commandables;
    private List<Direction> inputtedDirections;
    private DirectionCommandButton inputtedButton;
    public IUnityTimeService service = new UnityTimeService();
    public IUnityInputService inputService = new UnityInputService();
    public Action<T> onSelected;
    float timeOfLastInput;
    readonly float timeBeforeClearingInput;

    Action<T> IDirectionCommandPicker<T>.OnSelected { set => onSelected = value; }

    public DirectionCommandPicker(float timeBeforeClearingInput) {
        inputtedDirections = new List<Direction>(); 
        inputtedButton = DirectionCommandButton.NULL;
        this.timeBeforeClearingInput = timeBeforeClearingInput;
    }

    public T InputSelect()
    {
        if (commandables == null) throw new System.NullReferenceException("Commandables are null in DirectionCommandPicker. Did you call the Set method?");
        if (ExistingInput() && service.unscaledTime - timeOfLastInput > timeBeforeClearingInput)
        {
            clear();
        }
        Direction dir = DirectionalInput.GetSimpleDirection();

        if (inputtedDirections.Count <= 0)
        {
            if (dir != Direction.NULL)
            {
                inputtedDirections.Add(dir);
                timeOfLastInput = service.unscaledTime;
            }
        }
        else // If you've started inputting directions
        {
            // When a new button is pressed
            if (dir != inputtedDirections[inputtedDirections.Count - 1])
            {
                // if last dir is null and a new direction comes in replace the null
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

        if (inputService.GetKeyDown("z"))
        {
            inputtedButton = DirectionCommandButton.Z;
        }
        else if (inputService.GetKeyDown("x"))
        {
            inputtedButton = DirectionCommandButton.X;
        }
        else if (inputService.GetKeyDown("c"))
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
                onSelected?.Invoke(t);
                return t;
            }
        }
        return default;
    }

    public void Set(IEnumerable<T> commandables) {
        if (commandables == null) throw new ArgumentException("Trying to set commandables to null in DirectionCommandPicker");
        this.commandables = commandables;
        clear();
    }

    public void clear()
    {
        inputtedDirections.Clear();
        inputtedButton = DirectionCommandButton.NULL;
    }

    public bool ExistingInput()
    {
        return inputtedDirections.Count > 0 && !inputtedDirections.TrueForAll(x => x == Direction.NULL );
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
