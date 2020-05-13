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
                new DirectionCommand(DirectionCommandButton.Z, Direction.S),
                new DirectionCommand(DirectionCommandButton.Z, Direction.S, Direction.E),
                new DirectionCommand(DirectionCommandButton.X, Direction.W),
                new DirectionCommand(DirectionCommandButton.Z, Direction.S, Direction.S),
                new DirectionCommand(DirectionCommandButton.Z, Direction.S, Direction.W),
                new DirectionCommand(DirectionCommandButton.Z,Direction.E, Direction.S),
                new DirectionCommand(DirectionCommandButton.C, Direction.S, Direction.S, Direction.S),
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
