using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class RectangleArenaSetUp : MonoBehaviour
{
    public GameObject groundPrefab;
    //back wall, left wall, front wall, right wall
    public GameObject wallPrefab;
    private Wall[] walls;
    private Ground ground;

    public void Awake()
    {
        if (FindObjectOfType<Ground>() != null) return;
        ground = Instantiate(groundPrefab).GetComponent<Ground>();
        walls = new Wall[4];
        for (int i = 0; i < walls.Length; i++)
        {
            walls[i] = Instantiate(wallPrefab).GetComponent<Wall>();
            walls[i].transform.SetParent(ground.transform);
        }
        ground.transform.SetParent(transform);

        Vector3 groundColliderSize = Vector3.Scale(ground.GetComponent<BoxCollider>().size, ground.transform.localScale);
        for (int rotation = 0; rotation < 360; rotation += 90)
        {
            Wall wall = walls[rotation / 90];
            BoxCollider wallCollider = wall.GetComponent<BoxCollider>();
            float instantiateHeight = wall.transform.localScale.y * wallCollider.size.y * 2;

            float wallDistanceFromCentre;
            float wallWidth = 0;

            if (rotation == 0 || rotation == 180)
            {
                //xPos stays 0;
                wallDistanceFromCentre = groundColliderSize.z / 2;
                wallWidth = groundColliderSize.x;
            }
            else
            {
                // zPos stays 0;
                wallDistanceFromCentre = groundColliderSize.x / 2;
                wallWidth = groundColliderSize.z;
            }
            wallWidth += wallCollider.size.z;
            Quaternion wallRotation = Quaternion.Euler(0, rotation, 0);
            wall.transform.localPosition = wallRotation * new Vector3(0, instantiateHeight, wallDistanceFromCentre);
            wallCollider.size = new Vector3(wallWidth, wallCollider.size.y, wallCollider.size.z);
            wall.transform.localRotation = wallRotation;
        }
    }
}