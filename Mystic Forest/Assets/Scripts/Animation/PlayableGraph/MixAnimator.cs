using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class MixAnimator : MonoBehaviour, IMixAnimator
{

    IDictionary<string, PlayableAnimData> animMap;
    public IUnityTimeService service = new UnityTimeService();
    PlayableGraph playableGraph;
    AnimationMixerPlayable mixerPlayable;
    Coroutine coroutine;
    

    private struct PlayableAnimData
    {
        public readonly int pos;
        public readonly float speed;
        public readonly System.Func<float, Vector2> func;
        public readonly AnimationClipPlayable playable;
        public PlayableAnimData(int pos, float speed, System.Func<float, Vector2> func, AnimationClipPlayable playable)
        {
            this.pos = pos;
            this.speed = speed;
            this.func = func;
            this.playable = playable; 
        }
    }

    public void Initialize(IPlayableAnim[] anims)
    {
        animMap = new Dictionary<string, PlayableAnimData>();
        playableGraph = PlayableGraph.Create();
        AnimationPlayableOutput output = AnimationPlayableOutput.Create(playableGraph, "Animation", GetComponent<Animator>());
        mixerPlayable = AnimationMixerPlayable.Create(playableGraph, anims.Length, true);
        output.SetSourcePlayable(mixerPlayable);
        for (int i = 0; i < anims.Length; i++)
        {
            IPlayableAnim anim = anims[i];
            AnimationClipPlayable playable = AnimationClipPlayable.Create(playableGraph, anim.GetAnimationClip());
            playable.SetSpeed(anim.GetSpeed());
            animMap.Add(anim.GetName(),
               new PlayableAnimData(i, anim.GetSpeed(), anim.Evaluate, playable));
            playableGraph.Connect(playable, 0, mixerPlayable, i);
        }
        playableGraph.Play();
        GraphVisualizerClient.Show(playableGraph);
    }

    public void Play(string clipName)
    {
        if (!animMap.ContainsKey(clipName))
        {
            Debug.LogWarning(clipName);
            return;
        }
        foreach (KeyValuePair<string, PlayableAnimData> pair in animMap)
        {
            if (pair.Key == clipName)
            {
                pair.Value.playable.SetTime(0);
                mixerPlayable.SetInputWeight(pair.Value.pos, 1);
                
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
                coroutine = StartCoroutine(PlayMovement(pair.Value));
                
            } else
            {
                mixerPlayable.SetInputWeight(pair.Value.pos, 0);
            }
        }
    }

    IEnumerator PlayMovement(PlayableAnimData data)
    {
        Vector2 start = transform.position;
        float timePassed = 0;
        while (true)
        {
            yield return null;
            transform.position = start + data.func(timePassed);
            timePassed += Time.unscaledDeltaTime;
        }
    }

    void OnDestroy()
    {
        // Destroys all Playables and Outputs created by the graph.
        playableGraph.Destroy();
    }

    /* For Testing */

}
