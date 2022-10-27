using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    [SerializeField] Tilemap theMap;
    [SerializeField] bool isStationary;
    [SerializeField] bool dynamicMap;       // This is in place for now for randomly-gen'd maps

    GameObject player;
    Transform target;

    Vector3 bottomLeftLimit;
    Vector3 topRightLimit;

    float halfHeight;
    float halfWidth;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        target = player.transform;

        if (!dynamicMap)
        {
            halfHeight = Camera.main.orthographicSize;
            halfWidth = halfHeight * Camera.main.aspect;

            bottomLeftLimit = theMap.localBounds.min + new Vector3(halfWidth, halfHeight, 0f);
            topRightLimit = theMap.localBounds.max + new Vector3(-halfWidth, -halfHeight, 0f);

            // Send these bounds to the player
            player.GetComponent<PlayerController>().SetBounds(theMap.localBounds.min, theMap.localBounds.max);
        }
    }

    // LateUpdate is called once per frame after Update
    void LateUpdate()
    {
        if (!isStationary)
        {
            FollowPlayer();
        }
    }

    // PRIVATE FUNCTIONS

    // Function for attaching the camera to the player's position
    private void FollowPlayer()
    {
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);

        if (!dynamicMap)
        {
            // Keep the camera inside the bounds
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x),
                                             Mathf.Clamp(transform.position.y, bottomLeftLimit.y, topRightLimit.y),
                                             transform.position.z);
        }
    }

    // PUBLIC FUNCTIONS

    // Function that gets whether or not the camera is to be stationary for this scene
    public bool GetStationaryCam()
    {
        return isStationary;
    }

    // Function that gets whether or not the camera is on a dynamic map for this scene
    public bool GetDynamicMap()
    {
        return dynamicMap;
    }
}
