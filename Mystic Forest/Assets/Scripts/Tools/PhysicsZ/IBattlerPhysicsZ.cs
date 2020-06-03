using UnityEngine;
using System.Collections;

public interface IBattlerPhysicsZ : IPhysicsZ
{
    bool freeze { get; set; }
    bool lockZ { set; }
}
