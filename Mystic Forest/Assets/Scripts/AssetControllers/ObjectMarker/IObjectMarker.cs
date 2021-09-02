using UnityEngine;
using System.Collections;

public interface IObjectMarker
{

    void MarkObject(Transform @object);

    void UnmarkObject();
}
