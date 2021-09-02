using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zenject;

public class InputView : MonoBehaviour
{

    public KeyCode lightKey;
    public KeyCode heavyKey;
    public KeyCode jumpKey;


    public GameObject inputViewSectionPrefab;
    InputViewSection current;
    private InputViewSection.State prevState;
    public float up;
    List<InputViewSection> previousInputs;
    // public GameObject hoverTextPrefab;

    /*
    [Inject]
    public void Construct(IDirectionCommandPicker<IExecutableChain> picker)
    {
        picker.OnSelected.AddAction(HighLightSkill);
    }*/

    // Use this for initialization
    void Start()
    {
        current = Instantiate(inputViewSectionPrefab, transform).GetComponent<InputViewSection>();
        current.transform.localPosition = Vector3.zero;
        // current.UpdateState();
        previousInputs = new List<InputViewSection>();
        prevState = new InputViewSection.State();
    }
    /*
   void HighLightSkill(IExecutableChain chain)
   {



       if (!chain.IsSkill) return;
       IDirectionCommand command = chain.GetDirectionCommand();
       int j = command.directions.Length -1;
       bool commandFound = false;
       int i = previousInputs.Count - 1;
       for (; i >= 0 && j >= 0; i --)
       {
           InputViewSection s = previousInputs[i];
           if (!commandFound)
           {

               commandFound = (command.option == DirectionCommandButton.J && s.lightButton)
                   || (command.option == DirectionCommandButton.K && s.heavyButton);
           }
           if (commandFound && s.state.dir > -1)
           {
               Direction d = DirectionalInput.AngleToDirection(s.state.dir);
               if (d == command.directions[j])
               {
                   j --;
               }
           }
       }
       if (j < 0)
       {
           Debug.Log(i);                                                                                         SW);
       }
       }
       */


// Update is called once per frame
    void Update()
    {
        InputViewSection.State s = current.state.UpdateKey(lightKey, heavyKey, jumpKey);

        if (!prevState.Equals(current.state))
        {
            prevState = current.state;
            current.count = false;
            previousInputs.Add(current);
            previousInputs.ForEach((t) => t.transform.position += transform.rotation * (Vector3.up * up * transform.localScale.y));
            current = Instantiate(inputViewSectionPrefab, transform).GetComponent<InputViewSection>();
            current.transform.localPosition = Vector3.zero;
        }
        current.UpdateState(s);
    }

    
}
