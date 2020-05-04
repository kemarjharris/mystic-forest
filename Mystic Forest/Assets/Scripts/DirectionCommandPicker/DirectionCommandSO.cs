using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class DirectionCommandSO : ScriptableObject, IDirectionCommand
{
    public Direction[] Directions;
    public DirectionCommandButton Option;
    private DirectionCommandObjectOverrides overrides;
    public void Awake()
    {
        overrides = new DirectionCommandObjectOverrides(this);
    }

    DirectionCommandButton IDirectionCommand.option => Option;

    Direction[] IDirectionCommand.directions => Directions;

    public override bool Equals(object other) => overrides.Equals(other);

    public override int GetHashCode() => overrides.GetHashCode(Option, Directions);

    public override string ToString() => overrides.ToString(Option, Directions);
}