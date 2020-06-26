using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class VirtualHitCylinder : MonoBehaviour, IHitBox
{
    public void CheckCollision(Action<Collider> onCollide)
    {
        if (onCollide == null) return;
        List<Collider> overlapColliders = ObjectsInRange();
        for (int i = 0; i < overlapColliders.Count; i++)
        {
            onCollide?.Invoke(overlapColliders[i]);
        }
    }

    public Vector3 size;

    public List<Collider> ObjectsInRange()
    {
        Collider[] inBox = Physics.OverlapBox(transform.position, extents());
        List<Collider> inEllipseGameObjects = new List<Collider>();
        for (int i = 0; i < inBox.Length; i++)
        {
            if (inBox[i] != null && inEllipse(inBox[i].transform.position))
            {
                inEllipseGameObjects.Add(inBox[i]);
            }
        }
        return inEllipseGameObjects;
    }

    bool inEllipse(Vector3 point)
    {
        point -= transform.position;

        Vector3 extentVector = extents();
        // size along x axis
        float aRadiusSquared = Mathf.Pow(extentVector.x, 2);
        // size along y axis
        float bRadiusSquared = Mathf.Pow(extentVector.y, 2);
        // Point on ellipse
        float xSquared = Mathf.Pow(point.x, 2);
        float ySquared = Mathf.Pow(point.y, 2);

        return (xSquared * bRadiusSquared) + (ySquared * aRadiusSquared) <= (aRadiusSquared * bRadiusSquared);
    }

    Vector3 extents() => Vector3.Scale(size, transform.lossyScale) / 2;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, extents() * 2);
        int Segments = 32;
        Color Color = Color.blue;
        float XRadius = extents().x;
        float YRadius = extents().y;
        DrawEllipse(transform.position, XRadius, YRadius, Segments, Color, extents().z * 2);
    }

    private void DrawEllipse(Vector3 pos, float radiusX, float radiusY, int segments, Color color, float height, float duration = 0)
    {
        float angle = 0f;
        Quaternion rot = Quaternion.identity;
        Vector3 lastPoint = Vector3.zero;
        Vector3 thisPoint = Vector3.zero;

        for (int i = 0; i < segments + 1; i++)
        {
            thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX;
            thisPoint.z = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY;

            Vector3 topPoint = thisPoint + Vector3.right * height / 2;
            Vector3 bottomPoint = thisPoint - Vector3.right * height / 2;

            if (i > 0)
            {
                // top of ellipse
                Debug.DrawLine(rot * (lastPoint + (Vector3.right * height / 2)) + pos, rot * (thisPoint + (Vector3.right * height / 2)) + pos, color, duration);
                // bottoms of ellipse
                Debug.DrawLine(rot * (lastPoint - (Vector3.right * height / 2)) + pos, rot * (thisPoint - (Vector3.right * height / 2)) + pos, color, duration);
                // lines going horziontally on ellipse
                Debug.DrawLine(rot * topPoint + pos, rot * bottomPoint + pos, color, duration);
            }

            lastPoint = thisPoint;
            angle += 360f / segments;
        }
    }
}
