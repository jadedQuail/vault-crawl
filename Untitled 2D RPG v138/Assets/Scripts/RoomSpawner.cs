using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public int openingDirection;
    // 1 --> need bottom door
    // 2 --> need top door
    // 3 --> need left door
    // 4 --> need right door

    private RoomTemplates templates;
    private int rand;
    private bool spawned = false;

    BackgroundGenerator bgGenerator;

    private void Start()
    {
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        Invoke("Spawn", 0.1f);

        bgGenerator = GameObject.FindWithTag("DungeonManager").GetComponent<BackgroundGenerator>();
    }

    private void Spawn()
    {
        if (spawned == false)
        {
            if (openingDirection == 1)
            {
                // Need to spawn a room with a BOTTOM door.
                rand = Random.Range(0, templates.bottomRooms.Length);
                Instantiate(templates.bottomRooms[rand], transform.position, templates.bottomRooms[rand].transform.rotation);
                
                // Update min/max coordinates for the whole dungeon
                bgGenerator.UpdateMinMaxCoords(transform.position.x, transform.position.y);

                // These coordinates are now occupied; add it to current occupied coordinates
                bgGenerator.AddToOccupiedCoordinates(new Vector3(transform.position.x, transform.position.y, 0));
            }
            else if (openingDirection == 2)
            {
                // Need to spawn a room with a TOP door.
                rand = Random.Range(0, templates.topRooms.Length);
                Instantiate(templates.topRooms[rand], transform.position, templates.topRooms[rand].transform.rotation);

                // Update min/max coordinates for the whole dungeon
                bgGenerator.UpdateMinMaxCoords(transform.position.x, transform.position.y);

                // These coordinates are now occupied; add it to current occupied coordinates
                bgGenerator.AddToOccupiedCoordinates(new Vector3(transform.position.x, transform.position.y, 0));
            }
            else if (openingDirection == 3)
            {
                // Need to spawn a room with a LEFT door.
                rand = Random.Range(0, templates.leftRooms.Length);
                Instantiate(templates.leftRooms[rand], transform.position, templates.leftRooms[rand].transform.rotation);

                // Update min/max coordinates for the whole dungeon
                bgGenerator.UpdateMinMaxCoords(transform.position.x, transform.position.y);

                // These coordinates are now occupied; add it to current occupied coordinates
                bgGenerator.AddToOccupiedCoordinates(new Vector3(transform.position.x, transform.position.y, 0));
            }
            else if (openingDirection == 4)
            {
                // Need to spawn a room with a RIGHT door.
                rand = Random.Range(0, templates.rightRooms.Length);
                Instantiate(templates.rightRooms[rand], transform.position, templates.rightRooms[rand].transform.rotation);

                // Update min/max coordinates for the whole dungeon
                bgGenerator.UpdateMinMaxCoords(transform.position.x, transform.position.y);

                // These coordinates are now occupied; add it to current occupied coordinates
                bgGenerator.AddToOccupiedCoordinates(new Vector3(transform.position.x, transform.position.y, 0));
            }

            spawned = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SpawnPoint"))
        {
            // Destroy the spawner
            Destroy(gameObject);
        }
    }
}
