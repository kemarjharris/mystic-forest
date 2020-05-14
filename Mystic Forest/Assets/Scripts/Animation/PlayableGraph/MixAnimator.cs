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

    public void AddAnimation(IPlayableAnim anim)
    {
        mixerPlayable.SetInputCount(animMap.Count + 1);
        AnimationClipPlayable playable = AnimationClipPlayable.Create(playableGraph, anim.GetAnimationClip());
        playable.SetSpeed(anim.GetSpeed());
        animMap.Add(anim.GetName(),
           new PlayableAnimData(animMap.Count, anim.GetSpeed(), anim.Evaluate, playable));
        playableGraph.Connect(playable, 0, mixerPlayable, animMap.Count - 1);
    }

    public void Awake()
    {
        animMap = new Dictionary<string, PlayableAnimData>();
        playableGraph = PlayableGraph.Create();
        AnimationPlayableOutput output = AnimationPlayableOutput.Create(playableGraph, "Animation", GetComponent<Animator>());
        mixerPlayable = AnimationMixerPlayable.Create(playableGraph, 0, true);
        output.SetSourcePlayable(mixerPlayable);
        playableGraph.Play();
    }

    public void Play(IPlayableAnim anim)
    {
        string clipName = anim.GetName();
        if (!animMap.ContainsKey(clipName))
        {
            AddAnimation(anim);
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
