using UnityEngine;
using UnityEditor;

public interface IDirectionCommand
{
    DirectionCommandButton option { get; }

    Direction[] directions { get; }
}