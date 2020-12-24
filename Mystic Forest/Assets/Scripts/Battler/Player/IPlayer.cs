using UnityEngine;
using System.Collections;

public interface IPlayer : IBattler
{
    Transform target { get; set; }
}
