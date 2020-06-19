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
    Animator animator;
    

    private struct PlayableAnimData
    {
        public readonly int pos;
        public IPlayableAnim anim;
        public AnimationClipPlayable playable;
        public PlayableAnimData(int pos, IPlayableAnim anim, AnimationClipPlayable playable)
        {
            this.pos = pos;
            this.anim = anim;
            this.playable = playable;
        }
    }

    public void AddAnimation(IPlayableAnim anim)
    {
        mixerPlayable.SetInputCount(animMap.Count + 1);
        AnimationClipPlayable playable = AnimationClipPlayable.Create(playableGraph, anim.GetAnimationClip());
        playable.SetSpeed(anim.GetSpeed());
        animMap.Add(anim.GetName(),
           new PlayableAnimData(animMap.Count, anim, playable));
        playableGraph.Connect(playable, 0, mixerPlayable, animMap.Count - 1);
    }

    public void Awake()
    {
        animMap = new Dictionary<string, PlayableAnimData>();
        playableGraph = PlayableGraph.Create();
        animator = GetComponent<Animator>();
        AnimationPlayableOutput output = AnimationPlayableOutput.Create(playableGraph, "Animation", animator);
        mixerPlayable = AnimationMixerPlayable.Create(playableGraph, 0, true);
        output.SetSourcePlayable(mixerPlayable);
    }

    public void Stop()
    {
        playableGraph.Stop();
    }

    public void Play(IPlayableAnim anim)
    {
        string clipName = anim.GetName();
        if (!playableGraph.IsPlaying()) playableGraph.Play();
        if (!animMap.ContainsKey(clipName))
        {
            AddAnimation(anim);
        }
        foreach (KeyValuePair<string, PlayableAnimData> pair in animMap)
        {
            if (pair.Key == clipName)
            {
                animator.enabled = true;
                pair.Value.playable.SetTime(0);
                mixerPlayable.SetInputWeight(pair.Value.pos, 1);

                if (pair.Value.anim.Moves())
                {
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }
                    coroutine = StartCoroutine(PlayMovement(pair.Value));
                }
            } else
            {
                mixerPlayable.SetInputWeight(pair.Value.pos, 0);
            }
        }
    }

    IEnumerator PlayMovement(PlayableAnimData data)
    {
        Vector3 start = transform.position;
        float timePassed = 0;
        float length = data.anim.GetLength() / Mathf.Max(0.01f, data.anim.GetSpeed());
        while (timePassed < length)
        {
            yield return null;
            if (animator.enabled)
            {
                transform.position = start + data.anim.Evaluate(timePassed);
                timePassed += Time.unscaledDeltaTime;
            }
        }
    }

    void OnDestroy()
    {
        // Destroys all Playables and Outputs created by the graph.
        playableGraph.Destroy();
    }

    public void Pause()
    {
        animator.enabled = false;
    }

    public void Unpause()
    {
        animator.enabled = true;
    }

    /* For Testing */

}
