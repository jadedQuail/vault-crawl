using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaronLoader : MonoBehaviour
{
    List<GameObject> unusedSpawnNodes = new List<GameObject>();

    [Header("Min/Max Barons to Spawn")]
    [SerializeField] int minBarons = 1;
    [SerializeField] int maxBarons = 3;

    [Header("Baron GameObject")]
    [SerializeField] GameObject baronToSpawn;

    // List of all barons
    List<GameObject> baronList = new List<GameObject>();

    // Function that loads in barons to the dungeon (return # loaded)
    public int LoadBarons()
    {
        // If there are no unused spawn nodes, then don't spawn a baron
        if (unusedSpawnNodes.Count == 0) { return 0; }

        // Select the number of barons we're going to spawn
        int baronsToSpawn = Random.Range(minBarons, maxBarons + 1);

        for (int i = 0; i < baronsToSpawn; i++)
        {
            int unusedSpawnNodeIndex = Random.Range(0, unusedSpawnNodes.Count);
            GameObject nodeToSpawnAt = unusedSpawnNodes[unusedSpawnNodeIndex];
            Instantiate(baronToSpawn, nodeToSpawnAt.transform.position, Quaternion.identity);

            // Add this baron to the list
            baronList.Add(baronToSpawn);

            // Remove this spawn node after it's been used
            unusedSpawnNodes.RemoveAt(unusedSpawnNodeIndex);
        }

        return baronsToSpawn;
    }

    // Function that finds all the un-used spawn nodes in the dungeon
    public void FindUnusedSpawnNodes()
    {
        foreach (GameObject node in GameObject.FindGameObjectsWithTag("MobSpawner"))
        {
            // Unused spawner node
            if (!node.GetComponent<SpawnerNode>().GetIsUsed())
            {
                unusedSpawnNodes.Add(node);
            }
        }
    }

    // Function that removes a baron from the list of barons when it dies
    public void RemoveBaronFromList(GameObject baron)
    {
        if (baronList.Contains(baron))
        {
            baronList.Remove(baron);
        }
    }
}
