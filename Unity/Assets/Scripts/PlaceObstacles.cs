using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObstacles : MonoBehaviour
{
    [SerializeField] private GameObject[] randomWalls;
    private float[] possibleXValues = { -1.5f, 0.0f, 1.5f };
    
    void Start()
    {
        PlaceRandomWalls();
    }

    void Update()
    {
        
    }

    void PlaceRandomWalls()
    {
        foreach (GameObject randomWall in randomWalls)
        {
            float randomX = possibleXValues[Random.Range(0, possibleXValues.Length)];
            randomWall.transform.position =  new Vector3(randomX, randomWall.transform.position.y, randomWall.transform.position.z);
        }
    }
}
