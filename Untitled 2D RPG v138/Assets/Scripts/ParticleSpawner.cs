using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    [Header("Particle GameObject")]
    [SerializeField] GameObject particle;

    [Header("Spawn Interval Time")]
    [SerializeField] float timeBetweenParticles = 2f;
    float timeBetweenParticlesCounter;

    [Header("Spawn Territory")]
    [SerializeField] float xMin;
    [SerializeField] float xMax;
    [SerializeField] float yMin;
    [SerializeField] float yMax;

    [Header("Maximum Particles Allowed")]
    [SerializeField] float particleLimit;

    // Count of active particles
    int particleCount = 0;

    void Start()
    {
        // Initialize counter
        timeBetweenParticlesCounter = timeBetweenParticles;
    }

    void Update()
    {
        timeBetweenParticlesCounter -= Time.deltaTime;
        if (timeBetweenParticlesCounter <= 0 && particleCount < particleLimit)
        {
            // Randomly pick a spot for instantiation
            Vector2 spawnPos = new Vector2(Random.Range(xMin, xMax), Random.Range(yMin, yMax));

            // Instantiate a particle
            Instantiate(particle, spawnPos, particle.transform.rotation);

            // Increment the particle counter
            particleCount += 1;

            // Reset the counter
            timeBetweenParticlesCounter = timeBetweenParticles;
        }
    }

    // Returns the spawn territory for this particle spawner (only need max X and min Y values, as we're just looking for when to
    // despawn clouds at the bottom and right barriers)
    public (float, float) GetSpawnTerritory()
    {
        return (xMax, yMin);
    }

    // Increments the particle counter
    public void IncrementParticleCount(int amount)
    {
        particleCount += amount;
    }

    // Decrements the particle counter
    public void DecrementParticleCount(int amount)
    {
        particleCount -= amount;
    }
}
