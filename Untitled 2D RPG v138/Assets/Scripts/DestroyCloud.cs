using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCloud : MonoBehaviour
{
    // Interested in min Y value and max Y value for despawning clouds, specifically
    float xMax;
    float yMin;

    // Reference to particle spawner
    GameObject particleSpawner;

    // Start is called before the first frame update
    void Start()
    {
        // Find the particle spawner
        particleSpawner = GameObject.FindWithTag("ParticleSpawner");

        // Find spawn territory bounds
        (xMax, yMin) = particleSpawner.GetComponent<ParticleSpawner>().GetSpawnTerritory();
    }

    // Update is called once per frame
    void Update()
    {
        // Destroy the cloud if we're beyond the minimum Y or maximum X
        if (transform.position.x > xMax || transform.position.y < yMin)
        {
            // Despawn the cloud
            GetComponent<Animator>().SetTrigger("despawn");
        }
    }
}
