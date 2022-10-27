using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    // Room to replace faulty room, if detected
    public GameObject backupRoom;

    private bool collision = false;

    [SerializeField] GameObject closedNodeBackgroundPoint;

    private float timer = 1f;
    
    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            if (tag == "OpenNode" && collision == false)
            {
                // Instantiate a backup room with NO SPAWNERS
                Instantiate(backupRoom, transform.parent.position, transform.rotation);

                // Destroy this room (which is the parent)
                Destroy(transform.parent.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Solid Object"))
        {
            collision = true;
        }

        // If a closed and open node collide
        if (other.CompareTag("ClosedNode") && gameObject.CompareTag("OpenNode"))
        {
            // Instantiate a backup room with NO SPAWNERS
            Instantiate(backupRoom, transform.parent.position, transform.rotation);

            // Destroy this room (which is the parent)
            Destroy(transform.parent.gameObject);
        }
    }
}
