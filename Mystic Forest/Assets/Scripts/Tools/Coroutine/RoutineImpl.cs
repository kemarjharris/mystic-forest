using UnityEngine;
using System.Collections;

public class RoutineImpl : Routine
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
        running = false;
    }

    public bool IsFinishedRunning()
    {
        return started && !running; 
    }
}
