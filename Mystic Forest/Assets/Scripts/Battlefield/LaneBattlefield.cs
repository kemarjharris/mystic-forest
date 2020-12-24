using UnityEngine;
using UnityEditor;

public class LaneBattlefield : MonoBehaviour
{
    public Ground ground;
    public new BoxCollider collider;
    public int numberOfLanes;
    public float groundSize => (collider.size.z * transform.localScale.z);
    public float spaceBetweenLanes => groundSize / numberOfLanes;
    public float halfLane => spaceBetweenLanes / 2;
    public Vector3 bottomLeft => transform.position - new Vector3((collider.size.x / 2) * transform.localScale.x, 0, groundSize / 2) + new Vector3(0, (collider.size.y /2) * transform.localScale.y, 0);

    public float NthLane(int n)
    {
        if (n < 0 || n >= numberOfLanes) throw new System.ArgumentException(n + " is an invalid lane");
        return (spaceBetweenLanes / 2) + n * spaceBetweenLanes;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(bottomLeft, new Vector3(1,1,1));
    }
}