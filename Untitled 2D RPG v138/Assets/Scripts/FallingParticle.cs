using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingParticle : MonoBehaviour
{
    // Fall velocity of the particle
    [Header("Fall Velocity")]
    [SerializeField] float fallVelocityX = 0.4f;
    [SerializeField] float fallVelocityY = -0.4f;

    // Particle's Rigidbody2D
    Rigidbody2D particleRB;

    // Reference to the particleSpanwer
    GameObject particleSpawner;

    private void Start()
    {
        particleRB = GetComponent<Rigidbody2D>();
        particleRB.velocity = new Vector2(fallVelocityX, fallVelocityY);

        // Get the particle spawner
        particleSpawner = GameObject.FindWithTag("ParticleSpawner");
    }

    // Destroys the particle (called by particle's Animator)
    private void DestroyParticle()
    {
        // Decrement the number of active particles
        particleSpawner.GetComponent<ParticleSpawner>().DecrementParticleCount(1);

        // Destroy this object
        Destroy(gameObject);
    }

    // Destroys the particle once it goes out of bounds of the map
}
