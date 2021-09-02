using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zenject;

public class DirectionCommandPickerView : MonoBehaviour
{
    public GameObject sectionPrefab;
    private IDirectionCommandPicker<IExecutableChain> picker;
    private List<InputViewSection> inputs;

    [Inject]
    public void Construct(IDirectionCommandPicker<IExecutableChain> picker)
    {
        this.picker = picker;
    }

    public void Update()
    {
        Debug.Log(picker.heldDirection);
        
       // for (int i = 0; i < dirs.Length; i++)
        //{

        //}
    }

    public void OnInput(List<Direction> directions, DirectionCommandButton button)
    {
        CollectionUtils.Print(directions);
    }

    

}
