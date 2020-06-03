using UnityEngine;
using UnityEditor;

public abstract class ExecutableSO : ScriptableObject
{
    public DirectionCommandButton button;

    public bool isAerial;

    public abstract IExecutable CreateExecutable();

    private static AerialEvent aerial;

    private void OnEnable()
    {
        if (aerial == null) aerial = CreateInstance<AerialEvent>();
    }

    protected ExecutionEvent Instantiate(ExecutionEvent @event)
    {

        ExecutionEvent instance = Object.Instantiate(@event);
        if (isAerial)
        {
            // wrap event in an aerial
            AerialEvent aerialEvent = Object.Instantiate(aerial);
            aerialEvent.executionEvent = instance;
            // set returned instace to the wrapper
            instance = aerialEvent;
        }
        return instance;
    }

}