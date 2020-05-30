using UnityEngine;
using System.Collections;

public struct ActionWrapper<T> : IActionWrapper<T>
{

    System.Action<T> action;

    public void AddAction(System.Action<T> action) => this.action += action;
    public void RemoveAction(System.Action<T> action) => this.action -= action;
    public void Invoke(T obj) => action?.Invoke(obj);
    public static implicit operator System.Action<T>(ActionWrapper<T> wrapper) => wrapper.action;
}
