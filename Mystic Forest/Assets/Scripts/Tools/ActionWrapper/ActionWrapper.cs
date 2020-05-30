using UnityEngine;
using System.Collections;

public struct ActionWrapper : IActionWrapper
{

    System.Action action;

    public void AddAction(System.Action action) => this.action += action;
    public void RemoveAction(System.Action action) => this.action -= action;
    public void Invoke() => action?.Invoke();

    public static explicit operator System.Action (ActionWrapper wrapper) => wrapper.action;
}
