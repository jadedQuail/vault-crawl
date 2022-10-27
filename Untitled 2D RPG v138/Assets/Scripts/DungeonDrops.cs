using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonDrops : MonoBehaviour
{
    public GameObject[] drops;
    public int[] weights; // Must be between 0 and 1 (i.e. 0.50 is 50% of drops)
    public float noDropOdds; // Probability of nothing dropping

    // PUBLIC FUNCTIONS

    // Function for getting the list of drops for this dungeon, and the probability of each
    public (GameObject[], int[], float) GetDrops()
    {
        return (drops, weights, noDropOdds);
    }
}
