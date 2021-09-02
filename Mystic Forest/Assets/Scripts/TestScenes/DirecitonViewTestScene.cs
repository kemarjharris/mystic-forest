using UnityEngine;
using System.Collections;
using Zenject;

public class DirecitonViewTestScene : MonoBehaviour
{

    IDirectionCommandPicker<IExecutableChain> picker;

    [Inject]
    public void Construct(IDirectionCommandPicker<IExecutableChain> picker)
    {
        this.picker = picker;
    }

    // Update is called once per frame
    void Update()
    {
        picker.InputSelect();
    }
}
