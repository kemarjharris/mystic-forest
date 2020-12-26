using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class RectangleArenaSetUp : MonoBehaviour
{
    public GameObject groundPrefab;
    //back wall, left wall, front wall, right wall
    public GameObject wallPrefab;
    public Wall[] walls;
    public Ground ground;

    public void Awake()
    {
        if (FindObjectOfType<Ground>() != null) return;
        ground = Instantiate(groundPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Ground>();
        walls = new Wall[4];
        for (int i = 0; i < walls.Length; i++)
        {
            walls[i] = Instantiate(wallPrefab).GetComponent<Wall>();
            walls[i].transform.SetParent(ground.transform);
        }
        ground.transform.SetParent(transform);

        // update with values from prefab
        Vector3 groundVector = Vector3.Scale(ground.GetComponent<BoxCollider>().size, ground.transform.localScale);
        float wallHeight = walls[0].collider.size.y * walls[0].transform.localScale.y;
        UpdateArenaSize(new Vector3(groundVector.x, wallHeight, groundVector.z));
    }

    public void UpdateArenaSize(Vector3 size)
    {
        float xChange = size.x / ground.collider.size.x;
        float yChange = size.z / ground.collider.size.z;

        Vector3 scaleFactor = new Vector3(xChange, yChange, 1);
        ground.floor.transform.localScale = Vector3.Scale(scaleFactor, ground.floor.transform.localScale);
        ground.collider.size = new Vector3(size.x, ground.collider.size.y, size.z);

        for (int rotation = 0; rotation < 360; rotation += 90)
        {
            Wall wall = walls[rotation / 90];
            float instantiateHeight = wall.transform.localScale.y * wall.collider.size.y;

            float wallDistanceFromCentre;
            float wallWidth = 0;

            if (rotation == 0 || rotation == 180)
            {
                //xPos stays 0;
                wallDistanceFromCentre = size.z / 2;
                wallWidth = size.x;
            }
            else
            {
                // zPos stays 0;
                wallDistanceFromCentre = size.x / 2;
                wallWidth = size.z;
            }
            wallWidth += wall.collider.size.z;
            Quaternion wallRotation = Quaternion.Euler(0, rotation, 0);
            wall.transform.localPosition = (wallRotation * new Vector3(0, 0, wallDistanceFromCentre));// + Vector3.up * ground.collider.size.y;
            wall.collider.center = new Vector3(0, instantiateHeight / 2, 0);
            wall.collider.size = new Vector3(wallWidth, size.y, wall.collider.size.z);
            wall.transform.localRotation = wallRotation;
        }
    }
}