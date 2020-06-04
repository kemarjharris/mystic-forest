using UnityEngine;
using System.Collections;

public interface IBattlerPhysics : IPhysicsZ
{
    bool freeze { get; set; }
    bool lockZ { set; }
}
