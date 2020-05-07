using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public interface IDirectionCommandPicker<T>
{
    T InputSelect();
    void Set(IEnumerable<T> enumerable);
    System.Action<T> OnSelected { set; }
}