using System;
using UnityEngine;

public class DirectionCommand : IDirectionCommand {

    public Direction[] Directions;
    public DirectionCommandButton Option;
    private DirectionCommandObjectOverrides overrides;

    DirectionCommandButton IDirectionCommand.option => Option;

    Direction[] IDirectionCommand.directions => Directions;

    public DirectionCommand(DirectionCommandButton option, params Direction[] directions)
    {
        Option = option;
        Directions = directions;
        overrides = new DirectionCommandObjectOverrides(this);
    }

    public DirectionCommand(DirectionCommandButton option, IDirectionCommand command)
    {
        Option = option;
        Directions = command.directions;
        overrides = new DirectionCommandObjectOverrides(this);
    }

    public DirectionCommand(DirectionCommandButton option) : this(option, new Direction[0]){}

    public override bool Equals(object other) => overrides.Equals(other);

    public override int GetHashCode() => overrides.GetHashCode(Option, Directions);

    public override string ToString() => overrides.ToString(Option, Directions);

}
