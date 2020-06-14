using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionCommandPickerSceneTest : MonoBehaviour
{
    IDirectionCommandPicker<IDirectionPickable> picker;
    // Start is called before the first frame update
    void Start()
    {
        picker = new DirectionCommandPicker<IDirectionPickable>(1);
        List<IDirectionPickable> pickables = new List<IDirectionPickable>
            (new IDirectionPickable[] {
                new DirectionCommand( DirectionCommandButton.J, Direction.S),
                new DirectionCommand( DirectionCommandButton.J, Direction.S, Direction.E),
                new DirectionCommand( DirectionCommandButton.K, Direction.W),
                new DirectionCommand( DirectionCommandButton.J, Direction.S, Direction.S),
                new DirectionCommand( DirectionCommandButton.J, Direction.S, Direction.W),
                new DirectionCommand( DirectionCommandButton.J,Direction.E, Direction.S),
            });
        picker.Set(pickables);
    }

    // Update is called once per frame
    void Update()
    {
        IDirectionPickable pickable = picker.InputSelect();
        if (pickable != null) Debug.Log(pickable);
    }
}
