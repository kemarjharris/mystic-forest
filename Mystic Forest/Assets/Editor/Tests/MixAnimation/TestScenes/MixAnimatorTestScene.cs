using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixAnimatorTestScene : MonoBehaviour
{

    public PlayableAnimSO[] clips;
    public GameObject @object;
    IMixAnimator animator;

    // Start is called before the first frame update
    void Start()
    {
        MixAnimator animator = @object.GetComponent<MixAnimator>();
        this.animator = animator;
        IHitBox hitBox = @object.GetComponent<HitBox>();
    }

    // Update is called once per frame
    void Update()
    {
        string key = Input.inputString;
        if (key == "") return;
        try
        {
            int numPressed = int.Parse(key);
            animator.Play(clips[numPressed]);
        }
        catch
        {
            Debug.Log("Invalid key " + key);
        }
    }

}
