using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script for controlling an NPC's wandering movement
public class Wander : MonoBehaviour
{
    // "Script" for moving
    public string[] movementSequence;

    public float stepDistance;

    public float timeBetweenSteps;
    private float timer;
    private int stepCounter;

    public float npcSpeed = 10f;
    private Rigidbody2D npcRB;

    private Vector3 npcReferencePos;

    private bool canWander;
    private bool outOfDialogue;

    private Animator animator;

    private void Start()
    {
        npcRB = GetComponent<Rigidbody2D>();
        timer = timeBetweenSteps;

        // Set to negative 1 so that the first movement is a "pause"
        stepCounter = -1;

        animator = GetComponent<Animator>();

        npcReferencePos = transform.position;

        canWander = true;
        outOfDialogue = true;
    }

    // PRIVATE FUNCTIONS

    private void Update()
    {
        // Step manager - only step if we're not in dialogue and no menus are open
        if (outOfDialogue && !GameObject.FindWithTag("HUD").GetComponent<HUD>().AnyMenuOpen())
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0f)
        {
            canWander = true;
            timer = timeBetweenSteps;
            stepCounter++;
            if (stepCounter >= movementSequence.Length) // Reset the sequence
            {
                stepCounter = 0;
            }
        }
        TakeSteps();
    }

    // Function that moves the player via the next step
    private void TakeSteps()
    {
        if (stepCounter > -1) // Wait for the stepping to "begin"
        {
            // Move up
            if (movementSequence[stepCounter] == "up" && canWander)
            {
                if (transform.position.y > npcReferencePos.y + stepDistance) // Limit reached; stop moving
                {
                    npcRB.velocity = new Vector3(0f, 0f, 0f);
                    npcReferencePos = transform.position;
                    canWander = false;
                }
                else // Limit not reached; keep moving
                {
                    // Set animation to "up"
                    animator.SetFloat("directionY", 1);
                    animator.SetFloat("directionX", 0);

                    npcRB.velocity = new Vector3(0f, 10f, 0f);
                }
            }
            // Move down
            else if (movementSequence[stepCounter] == "down" && canWander)
            {
                if (transform.position.y < npcReferencePos.y - stepDistance) // Limit reached; stop moving
                {
                    npcRB.velocity = new Vector3(0f, 0f, 0f);
                    npcReferencePos = transform.position;
                    canWander = false;
                }
                else // Limit not reached; keep moving
                {
                    // Set animation to "down"
                    animator.SetFloat("directionY", -1);
                    animator.SetFloat("directionX", 0);

                    npcRB.velocity = new Vector3(0f, -10f, 0f);
                }
            }
            // Move right
            else if (movementSequence[stepCounter] == "right" && canWander)
            {
                if (transform.position.x > npcReferencePos.x + stepDistance) // Limit reached; stop moving
                {
                    npcRB.velocity = new Vector3(0f, 0f, 0f);
                    npcReferencePos = transform.position;
                    canWander = false;
                }
                else // Limit not reached; keep moving
                {
                    // Set animation to "right"
                    animator.SetFloat("directionY", 0);
                    animator.SetFloat("directionX", 1);

                    npcRB.velocity = new Vector3(10f, 0f, 0f);
                }
            }
            // Move left
            else if (movementSequence[stepCounter] == "left" && canWander)
            {
                if (transform.position.x < npcReferencePos.x - stepDistance) // Limit reached; stop moving
                {
                    // Set animation to "left"
                    animator.SetFloat("directionY", 0);
                    animator.SetFloat("directionX", -1);

                    npcRB.velocity = new Vector3(0f, 0f, 0f);
                    npcReferencePos = transform.position;
                    canWander = false;
                }
                else // Limit not reached; keep moving
                {
                    npcRB.velocity = new Vector3(-10f, 0f, 0f);
                }
            }
        }
    }

    // PUBLIC FUNCTIONS
    
    // Function that starts/stops the NPC from wandering
    public void ToggleNPCWandering(bool value)
    {
        outOfDialogue = value;
    }
}
