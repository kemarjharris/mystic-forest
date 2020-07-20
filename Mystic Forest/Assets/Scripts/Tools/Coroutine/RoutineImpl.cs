using UnityEngine;
using System.Collections;

public class RoutineImpl : IRoutine
{

    public RoutineImpl(IEnumerator enumerator, MonoBehaviour runner, bool alreadyRunning = false)
    {
        this.runner = runner;
        this.enumerator = enumerator;
        running = alreadyRunning;
    }

    MonoBehaviour runner;
    public IEnumerator enumerator;
    public bool running;
    bool started = false;
    public System.Action OnRoutineFinished { get; set; }

    public object Current
    {
        get
        {
            return enumerator.Current;
        }
    }

    public bool IsRunning()
    {
        return running;
    }

    public bool MoveNext()
    {
        //Debug.Log("ive got here!");
        started = true;
        running = enumerator.MoveNext();
        if (!running)
        {
            OnRoutineFinished?.Invoke();
        }
        return running;
    }

    public void Reset()
    {
        enumerator.Reset();
        running = false;
    }

    public void Start()
    {
        started = true;
        runner.StartCoroutine(this);
    }

    public void Stop()
    {
        runner.StopCoroutine(this);
        OnRoutineFinished?.Invoke();
        running = false;
    }

    public bool IsFinishedRunning()
    {
        return started && !running; 
    }
}
