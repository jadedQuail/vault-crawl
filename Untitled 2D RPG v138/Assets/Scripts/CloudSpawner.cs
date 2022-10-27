using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [Header("Cloud Object")]
    [SerializeField] GameObject cloud;

    [Header("Time Between Spawns")]
    [SerializeField] float timeBetweenSpawns;
    float timeBetweenSpawnsCounter;

    void Start()
    {
        // Initialize the counter
        timeBetweenSpawnsCounter = timeBetweenSpawns;
    }

    void Update()
    {
        timeBetweenSpawnsCounter -= Time.deltaTime;
        if (timeBetweenSpawnsCounter < 0f)
        {
            Instantiate(cloud, transform.position, cloud.transform.rotation);
            timeBetweenSpawnsCounter = timeBetweenSpawns;
        }
    }
}
