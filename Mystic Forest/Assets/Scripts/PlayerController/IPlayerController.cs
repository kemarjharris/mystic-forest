using UnityEngine;
using System.Collections;

public interface IPlayerController
{
    void Update();

    void FixedUpdate();

    void OnEnable();

    void OnDisable();
}
