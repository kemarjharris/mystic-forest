using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DirectionCommandObjectOverrides
{
    IDirectionCommand command;
    
    public DirectionCommandObjectOverrides(IDirectionCommand command)
    {
        this.command = command;
    }

    public override bool Equals(object other)
    {
        if (other == null) return false;
        return other.GetHashCode() == GetHashCode();
    }

    public override int GetHashCode() => GetHashCode(command.option, command.directions);

    public int GetHashCode(DirectionCommandButton option, Direction[] directions)
    {
        int hash = 1;
        for (int i = 0; i < 3 && i < directions.Length; i++)
        {
            hash += (int)((((int)directions[i]) + 1) * (UnityEngine.Mathf.Pow(10f, i)));
        }

        if (option == DirectionCommandButton.Z)
        {
            hash *= 17;
        }
        else if (option == DirectionCommandButton.X)
        {
            hash *= 19;
        }
        else if (option == DirectionCommandButton.C)
        {
            hash *= 13;
        }
        else
        {
            hash *= -1;
        }

        if (directions.Length > 0)
        {
            hash /= (directions.Length * 11);
        }


        return hash;
    }

    public string ToString(DirectionCommandButton option, Direction[] directions)
    {
        string s = "";

        for (int i = 0; i < directions.Length; i++)
        {
            switch (directions[i])
            {
                case Direction.W:
                    s += '\u2190';
                    break;
                case Direction.N:
                    s += '\u2191';
                    break;
                case Direction.E:
                    s += '\u2192';
                    break;
                case Direction.S:
                    s += '\u2193';
                    break;
                case Direction.NW:
                    s += '\u2196';
                    break;
                case Direction.NE:
                    s += '\u2197';
                    break;
                case Direction.SE:
                    s += '\u2198';
                    break;
                case Direction.SW:
                    s += '\u2199';
                    break;
                default:
                    s += "";
                    break;
            }
        }

        switch (option)
        {
            case DirectionCommandButton.Z:
                s += "Z";
                break;
            case DirectionCommandButton.C:
                s += "C";
                break;
            case DirectionCommandButton.X:
                s += "X";
                break;
        }

        return s;
    }
}