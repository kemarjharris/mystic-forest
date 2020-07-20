using UnityEngine;
using System.Collections;

public static class TestTools
{
    public static void DestroyAllGameObjects()
    {
        GameObject[] GameObjects = Object.FindObjectsOfType<GameObject>() as GameObject[];

        for (int i = 0; i < GameObjects.Length; i++)
        {
            Object.Destroy(GameObjects[i]);
        }
    }
}
