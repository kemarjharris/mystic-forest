using UnityEngine;
using UnityEditor;

public interface IDirectionCommand : IDirectionPickable
{
    DirectionCommandButton option { get; }

    Direction[] directions { get; }
}