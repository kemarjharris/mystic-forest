using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

using UnityEngine.Animations;

[RequireComponent(typeof(Animator))]
public class MixAnimation : MonoBehaviour
{
    public AnimationClip[] clips;

    [Range(0, 1)]
    public float weight;
    private float weightLastFrame = 0;
    PlayableGraph playableGraph;

    AnimationMixerPlayable mixerPlayable;

    List<AnimationClipPlayable> playables;

    void Start()

    {

        // Creates the graph, the mixer and binds them to the Animator.

        playableGraph = PlayableGraph.Create();

        var playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", GetComponent<Animator>());

        mixerPlayable = AnimationMixerPlayable.Create(playableGraph, clips.Length, true);

        playableOutput.SetSourcePlayable(mixerPlayable);

        // Creates AnimationClipPlayable and connects them to the mixer.


        /*
        List<AnimationClipPlayable> clipPlayables = new List<AnimationClipPlayable>();
        for (int i = 0; i < clips.Length; i++) clipPlayables.Add(AnimationClipPlayable.Create(playableGraph, clips[i]));
        for (int i = 0; i < clipPlayables.Count; i++) playableGraph.Connect(clipPlayables[i], 0, mixerPlayable, i);
        */
        //var clipPlayable0 = AnimationClipPlayable.Create(playableGraph, clips[0]);

        //var clipPlayable1 = AnimationClipPlayable.Create(playableGraph, clips[1]);

        //var clipPlayable2 = AnimationClipPlayable.Create(playableGraph, clips[2]);

        //playableGraph.Connect(clipPlayable0, 0, mixerPlayable, 0);

        //playableGraph.Connect(clipPlayable1, 0, mixerPlayable, 1);

        //playableGraph.Connect(clipPlayable2, 0, mixerPlayable, 2);


        playables = new List<AnimationClipPlayable>();

        for (int i = 0; i < clips.Length; i++)
        {
            var clip = AnimationClipPlayable.Create(playableGraph, clips[i]);
            playables.Add(clip);
            //clip.SetSpeed(0.33f);
            playableGraph.Connect(clip, 0, mixerPlayable, i);
        }

        // Plays the Graph.

        playableGraph.Play();

        GraphVisualizerClient.Show(playableGraph);

    }

    void Update()

    {
        

        float newWeight = Mathf.Clamp01(weight);
        // if (newWeight == weight) return;
        for (int i = 0; i < clips.Length; i ++)
        {
            float inputWeight = 0;
            if (i <= weight * clips.Length && weight * clips.Length <= i + 1) {
                
                
                    inputWeight = 1;
                    if (weight != weightLastFrame)
                    {
                        Debug.Log("ARE PEE JEE");
                        playables[i].SetTime(0);
                        weight = newWeight;
                    }
                
            }
            // if the weight is within the given range, play that clip, otherwise play nothing
            mixerPlayable.SetInputWeight(i, inputWeight);
        }
        weightLastFrame = weight;

        

    }

    void OnDisable()

    {

        // Destroys all Playables and Outputs created by the graph.

        playableGraph.Destroy();

    }

}
