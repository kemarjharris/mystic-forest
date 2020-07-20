using UnityEngine;
using System.Collections;

public interface IRoutine : IEnumerator
{

    // Use this for initialization
    void Start();

    // Update is called once per frame
    void Stop();

    bool IsRunning();

    bool IsFinishedRunning();

    System.Action OnRoutineFinished { get; set; }
}
