using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounty : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] Animator enemyAnim;

    [Header("")]
    [SerializeField] float movementTime = 3f;
    float movementTimer;

    // This is the speed that matters for bounty enemies
    [SerializeField] float bountyEnemySpeed = 14f;

    // Self references
    Rigidbody2D enemyRB;

    // Stores old velocity when there's a pause
    Vector2 storedVelocity;
    bool bountyUnpaused;


    int[] directions = new int[] { -1, 1 };

    // Start is called before the first frame update
    void Start()
    {
        // Initialize movementTimer
        movementTimer = movementTime;

        // Initialize references
        enemyRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<EnemyController>().GetCanMove()) 
        {
            // When the bounty enemy can move again, give it its last stored velocity
            if (bountyUnpaused)
            {
                enemyRB.velocity = storedVelocity;
                bountyUnpaused = false;
            }

            // Bounty enemy moves randomly and erratically
            MoveRandomly(); 
        }
        else
        {
            // If the enemy can't move, freeze its velocity
            enemyRB.velocity = new Vector2(0f, 0f);
            bountyUnpaused = true;
        }
    }

    // Function that has the bounty enemy move randomly
    private void MoveRandomly()
    {
        movementTimer -= Time.deltaTime;

        if (movementTimer <= 0f)
        {
            // Generate a new random direction
            Vector2 randomDirection = new Vector2(directions[Random.Range(0, 2)], directions[Random.Range(0, 2)]);

            // Set the enemy's velocity
            enemyRB.velocity = Vector3.Normalize(randomDirection) * bountyEnemySpeed;
            storedVelocity = enemyRB.velocity;  // Constantly store this velocity, for re-use later after game pause is over

            // Adjust animations
            enemyAnim.SetFloat("moveX", randomDirection.x);
            enemyAnim.SetFloat("moveY", randomDirection.y);

            // Reset the movement timer
            movementTimer = movementTime;
        }
    }
}
