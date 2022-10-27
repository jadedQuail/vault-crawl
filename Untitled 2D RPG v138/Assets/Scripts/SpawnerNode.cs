using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerNode : MonoBehaviour
{
    private bool isUsed = false;

    // Function that gets whether or not this spawner node has been used
    public bool GetIsUsed()
    {
        return isUsed;
    }

    // Function that sets whether or not this spawner node has been used
    public void SetIsUsed(bool value)
    {
        isUsed = value;
    }
}
