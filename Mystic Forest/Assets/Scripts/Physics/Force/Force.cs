using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[System.Serializable]
public class Force : IForce
{
    public Vector2 direction;
    public ForceMode mode;
    public Until[] applyUntil = new Until[1];


    public IEnumerator ApplyForce(IBattlerPhysics physics, Transform applier)
    {
        ClassVector forceVector = new ClassVector();
        yield return SetForceVector(physics, applier, forceVector);

        do
        {
            physics.SetVelocity(applier.YRotate(forceVector.v));
            yield return new WaitForFixedUpdate();
        } while (!UntilConditionMet(physics));
    }

    public bool UntilConditionMet(IBattlerPhysics physics)
    {
        if (applyUntil.Length == 1 && applyUntil[0] == Until.ONE_FRAME_PASSES)
        {
            return true;
        } else
        {
            bool met = false;
            for (int i = 0; i < applyUntil.Length; i++)
            {
                switch (applyUntil[i])
                {
                    case Until.HITS_WALL:
                        met = UntilFunction.HitsWall(physics);
                        break;
                        
                }
                if (met)
                {
                    return true;
                }
            }
            return met;
        }
    }

    public IEnumerator SetForceVector(IBattlerPhysics physics, Transform forceOrigin, ClassVector forceVector)
    {
        switch (mode)
        {
            case ForceMode.DIRECTION:
                forceVector.v = direction;
                break;
            case ForceMode.VELOCITY:
                yield return CalculateAndSetVelocity(forceOrigin, forceVector);
                break;
            case ForceMode.OUTWARDS:
                Vector3 temp = CalculateOutwardForce(physics, forceOrigin);
                forceVector.v = temp;
                break;
        }
    }

    public IEnumerator CalculateAndSetVelocity(Transform transform, ClassVector forceVector)
    {
        Vector3 previousPosition = transform.position;
        yield return new WaitForFixedUpdate();
        Vector3 position = transform.position;
        Vector3 velocity = (position - previousPosition) / Time.fixedDeltaTime;
        forceVector.v = velocity;
    }

    public Vector3 CalculateOutwardForce(IBattlerPhysics physics, Transform origin)
    {

        float z = physics.transform.position.z - origin.position.z;
        float x = physics.transform.position.x - origin.position.x;

        float radians = Mathf.Atan2(z, x);
        Vector3 modifiedVector = new Vector3(direction.x * Mathf.Cos(radians), direction.y, direction.x * Mathf.Sin(radians));
        return modifiedVector;
    }

     
    public enum ForceMode
    {
        DIRECTION, VELOCITY, OUTWARDS
    }

    public enum Until
    {
        ONE_FRAME_PASSES, HITS_WALL
    }

    private class UntilFunction
    {
        public static bool HitsWall(IBattlerPhysics physics) {
            Wall[] walls = Object.FindObjectsOfType<Wall>();
            for (int i = 0; i < walls.Length; i++)
            {
                if (walls[i].IsTouching(physics.transform.gameObject))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class ClassVector
    {
        public Vector3 v;
    }
}
