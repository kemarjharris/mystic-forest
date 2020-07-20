using UnityEngine;
using System.Collections;

public interface IForce
{
    IEnumerator ApplyForce(IBattlerPhysics physics, Transform applier);
}
