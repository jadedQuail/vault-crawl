using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropController : MonoBehaviour
{
    // List that contains all the drops
    [SerializeField] GameObject[] allDrops;

    // PUBLIC FUNCTIONS

    // Function that returns a random pickup
    public GameObject GetRandomPickup()
    {
        return allDrops[Random.Range(0, allDrops.Length)];
    }

    // Function that returns a random pickup from a given list
    public GameObject GetRandomPickupFromList(GameObject[] pickups)
    {
        return pickups[Random.Range(0, pickups.Length)];
    }

    // Function that returns a specific pickup
    public GameObject GetPickup(string pickupName)
    {
        // Go through each drop and find the one that matches, then drop it
        foreach (GameObject drop in allDrops)
        {
            if (drop.GetComponent<Weapon>().GetWeaponName() == pickupName)
            {
                return drop;
            }
        }

        // Failed to find the drop
        return null;
    }

    // Function that gets a random drop from a list, factoring in probabilities
    public GameObject GeneratePickupWithWeights(GameObject[] drops, int[] weights, float noDropOdds)
    {
        // NOTE: THE WEIGHTS HAVE TO BE IN DESCENDING ORDER

        // First calculate if something is going to drop
        float dropIndicator = Random.Range(0f, 1f);
        if (dropIndicator <= noDropOdds)
        {
            return null;
        }

        // Total all the "weights"
        int totalWeight = 0;
        {
            foreach (int weight in weights)
            {
                totalWeight += weight;
            }
        }

        // Generate a random number between 0 and the totalWeight
        int rngValue = Random.Range(0, totalWeight);

        for (int i=0; i < drops.Length; i++)
        {
            if (rngValue < weights[i])
            {
                return drops[i];
            }
            else
            {
                rngValue -= weights[i];
            }
        }

        // Something went wrong if we get to this return statement
        return null;
    }

    // Function that drops a coin at the enemy's location
    public void DropCoin(GameObject enemy)
    {
        // Reference to the EnemyController on this enemy
        EnemyController enemyController = enemy.GetComponent<EnemyController>();

        if (!enemyController.GetDropsClusterCoins())
        {
            // Instantiate one coin
            Instantiate(enemyController.GetCoin(), enemy.transform.position, enemy.transform.rotation).GetComponent<Coin>().SetCoinValue(enemyController.GetCoinValue());
        }
        else
        {
            // Instantiate 5 coins (or stacks)

            // Four offsets
            Vector3 north = new Vector3(enemy.transform.position.x, enemy.transform.position.y + 0.5f, enemy.transform.position.z);
            Vector3 south = new Vector3(enemy.transform.position.x, enemy.transform.position.y - 0.5f, enemy.transform.position.z);
            Vector3 east = new Vector3(enemy.transform.position.x + 0.5f, enemy.transform.position.y, enemy.transform.position.z);
            Vector3 west = new Vector3(enemy.transform.position.x - 0.5f, enemy.transform.position.y, enemy.transform.position.z);

            Instantiate(enemyController.GetCoin(), enemy.transform.position, enemy.transform.rotation).GetComponent<Coin>().SetCoinValue(enemyController.GetCoinValue());
            Instantiate(enemyController.GetCoin(), north, enemy.transform.rotation).GetComponent<Coin>().SetCoinValue(enemyController.GetCoinValue());
            Instantiate(enemyController.GetCoin(), south, enemy.transform.rotation).GetComponent<Coin>().SetCoinValue(enemyController.GetCoinValue());
            Instantiate(enemyController.GetCoin(), east, enemy.transform.rotation).GetComponent<Coin>().SetCoinValue(enemyController.GetCoinValue());
            Instantiate(enemyController.GetCoin(), west, enemy.transform.rotation).GetComponent<Coin>().SetCoinValue(enemyController.GetCoinValue());
        }
    }
}
