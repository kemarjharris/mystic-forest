using UnityEngine;
using System.Collections;
using Zenject;

public class AssetSpawner : MonoBehaviour
{
    DiContainer spawner;
    public GameObject[] objectsToSpawn;

    [Inject] 
    public void Construct(DiContainer spawner)
    {
        this.spawner = spawner;
    }

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < objectsToSpawn.Length; i++)
        {
            spawner.InstantiatePrefab(objectsToSpawn[i]);
        }
    }
}
