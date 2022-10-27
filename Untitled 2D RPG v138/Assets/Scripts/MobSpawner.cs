using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    public GameObject[] mobSpawners;
    public GameObject[] mobs;

    public int minMobsToSpawn;
    public int maxMobsToSpawn;

    private List<int> usedSpawners = new List<int>();

    private void Start()
    {
        SpawnMobs();
    }

    // Function that spawns mobs for the particular tile it is attached to
    private void SpawnMobs()
    {
        // Randomly select how many mobs to spawn
        // (Maximum is the # of spawners, b/c 1 per spawner)
        int mobsToSpawn = Random.Range(minMobsToSpawn, maxMobsToSpawn);
        
        // Spawn that number of mobs
        for (int i = 0; i < mobsToSpawn; i++)
        {
            int spawner;
            spawner = Random.Range(0, mobSpawners.Length);

            // If the spawner is used, keep regenerating until an unused spawner appears
            while (usedSpawners.Contains(spawner))
            {
                spawner = Random.Range(0, mobSpawners.Length);
            }

            // Instantiate the mob
            Instantiate(mobs[Random.Range(0, mobs.Length)], mobSpawners[spawner].transform.position, Quaternion.identity);

            // Mark the spawner as used
            // Add each used spawner to the total list
            if (!usedSpawners.Contains(spawner))
            {
                usedSpawners.Add(spawner);
                mobSpawners[spawner].GetComponent<SpawnerNode>().SetIsUsed(true);
            }
        }
    }
}
