using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trailer : MonoBehaviour
{
    [SerializeField] GameObject trail;
    [SerializeField] float timeBetweenTrails;
    [SerializeField] float trailSize;
    [SerializeField] float instantiationGap;

    float betweenTimer;
    float sizeTimer;
    float instantiationTimer;

    // GameObject references
    GameObject hud;
    GameObject abilityManager;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize HUD reference
        hud = GameObject.FindWithTag("HUD");
        abilityManager = GameObject.FindWithTag("AbilityManager");

        // Initialize timer values
        betweenTimer = timeBetweenTrails;
        sizeTimer = trailSize;
        instantiationTimer = instantiationGap;
    }

    // Update is called once per frame
    void Update()
    {
        // Countdown when player is in spawn range
        if (GetComponent<EnemyController>().GetInInteractRange())
        {
            // Don't decrement the timer if:
            // 1.) The trailer is frozen by ability
            // 2.) Any menu is open
            // 3.) The player is invisible
            // 4.) The trailer isn't moving
            if (!GetComponent<EnemyController>().GetFrozenByAbility() && !hud.GetComponent<HUD>().AnyMenuOpen()
                && !abilityManager.GetComponent<Invisibility>().GetIsInvisible() && GetComponent<Rigidbody2D>().velocity.magnitude != 0f)
            {
                betweenTimer -= Time.deltaTime;
            }
        }
        else
        {
            betweenTimer = timeBetweenTrails;
        }

        if (betweenTimer <= 0f)  // Timer has reached the end
        {
            instantiationTimer -= Time.deltaTime;
            if (instantiationTimer <= 0f)
            {
                // Instantiate trail (and have it point back to this trailer)
                Instantiate(trail, transform.position, transform.rotation).GetComponent<Trail>().SetEnemySpawnedFrom(gameObject);
                instantiationTimer = instantiationGap;
            }

            sizeTimer -= Time.deltaTime;
            if (sizeTimer <= 0f)
            {
                // Reset both
                betweenTimer = timeBetweenTrails;
                sizeTimer = trailSize;
            }
        }
    }
}
