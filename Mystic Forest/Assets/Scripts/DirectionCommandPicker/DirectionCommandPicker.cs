using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Zenject;

public class DirectionCommandPicker<T> : IDirectionCommandPicker<T> where T : IDirectionPickable
{
    // Dependencies
    public IUnityTimeService service;
    public IUnityInputService inputService;
    readonly float timeBeforeClearingInput;

    // Events
    public IActionWrapper<T> OnSelected { get; private set; }
    public Action<List<Direction>, DirectionCommandButton> OnNewInput { get; set; }
    public Direction heldDirection { get; private set; }

    // State
    private IEnumerable<T> commandables;
    private List<Direction> inputtedDirections;
    private DirectionCommandButton inputtedButton;
    float timeOfLastInput;
    
    [Inject]
    public void Construct(IUnityTimeService service, IUnityInputService inputService)
    {
        this.service = service;
        this.inputService = inputService;
    }

    public DirectionCommandPicker(float timeBeforeClearingInput) {
        inputtedDirections = new List<Direction>(); 
        inputtedButton = DirectionCommandButton.NULL;
        this.timeBeforeClearingInput = timeBeforeClearingInput;
        OnSelected = new ActionWrapper<T>();
    }

    public T InputSelect()
    {

        if (commandables == null) throw new NullReferenceException("Commandables are null in DirectionCommandPicker. Did you call the Set method?");
        if (ExistingInput() && service.unscaledTime - timeOfLastInput > timeBeforeClearingInput)
        {
            clear();
        }
        Direction dir = DirectionalInput.GetSimpleDirection();
        heldDirection = dir;

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

        if (inputService.GetKeyDown("j"))
        {
            inputtedButton = DirectionCommandButton.J;
        }
        else if (inputService.GetKeyDown("k"))
        {
            inputtedButton = DirectionCommandButton.K;
        }

        T t = default;

        if (inputtedButton != DirectionCommandButton.NULL)
        {
            // remove placeholder null tacked on the end of the list
            if (inputtedDirections.Count > 0 && inputtedDirections[inputtedDirections.Count - 1] == Direction.NULL)
            {
                inputtedDirections.RemoveAt(inputtedDirections.Count - 1);
            }
            int count = inputtedDirections.Count;

            for (int i = 0; i <= count; i ++)
            {
                Direction[] directions = inputtedDirections.ToArray();
                t = Select(new DirectionCommand(inputtedButton, directions));
                if (t == null)
                {
                    t = Select(new DirectionCommand(DirectionCommandButton.J_OR_K, directions));
                }
                if (t != null // valid chain picked
                    && (inputtedDirections.Count != 1 // chain was not a single direction chain
                    || inputtedDirections.Count == 1 && dir == inputtedDirections[0])) // chain was a dingle button chain and dir was held this frame
                {
                    Debug.Log(t);
                    clear();
                    OnSelected.Invoke(t);
                    return t;
                }
                if (inputtedDirections.Count > 0)
                {
                    inputtedDirections.RemoveAt(0);
                }
            }
           
            clear();
        }
        return default;
    }

    public void Set(IEnumerable<T> commandables) {
        this.commandables = commandables ?? throw new ArgumentException("Trying to set commandables to null in DirectionCommandPicker");
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
            //

            if (pickable.GetDirectionCommand().GetHashCode() == hash)
            {
                return (T) pickable;
            }
        }
        return default;
    }
}
