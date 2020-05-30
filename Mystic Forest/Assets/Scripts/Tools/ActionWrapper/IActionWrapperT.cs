using UnityEngine;
using System.Collections;

public interface IActionWrapper<T> 
{
    void AddAction(System.Action<T> action);

    void RemoveAction(System.Action<T> action);

    void Invoke(T obj);
}
