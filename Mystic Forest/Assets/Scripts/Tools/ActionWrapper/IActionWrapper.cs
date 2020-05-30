using UnityEngine;
using System.Collections;

public interface IActionWrapper
{
    void AddAction(System.Action action);

    void RemoveAction(System.Action action);

    void Invoke();
}
