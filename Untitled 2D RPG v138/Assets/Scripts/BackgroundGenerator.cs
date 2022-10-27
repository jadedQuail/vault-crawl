using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script generates backgrounds for the dungeon
public class BackgroundGenerator : MonoBehaviour
{
    // The background room to spawn
    public GameObject backgroundRoom;

    // Time to wait to generate the background
    public float timeUntilGeneration = 1.5f;

    private bool backgroundGenerated = false;

    // Coordinates for minimum and maximum of the randomly-generated map
    private float xMin = 0;
    private float xMax = 0;
    private float yMin = 0;
    private float yMax = 0;
    private Vector3 botLeftCoord;
    private Vector3 topRightCoord;

    // List of coordinates that are unavailable for background spawning in dungeons
    List<Vector3> occupiedCoords = new List<Vector3>();

    private void Update()
    {
        // Generate the background after some time
        timeUntilGeneration -= Time.deltaTime;
        if (timeUntilGeneration <= 0f && !backgroundGenerated) 
        { 
            GenerateBackground();
            backgroundGenerated = true;
        }
    }

    // Public Functions //

    // Function that updates the bottom left and right coordinates
    public void UpdateMinMaxCoords(float xVal, float yVal)
    {
        // New minimum for X
        if (xVal < xMin)
        {
            xMin = xVal;
        }

        // New maximum for X
        if (xVal > xMax)
        {
            xMax = xVal;
        }

        // New minimum for Y
        if (yVal < yMin)
        {
            yMin = yVal;
        }

        // New maximum for Y
        if (yVal > yMax)
        {
            yMax = yVal;
        }

        // Calculate new coordinates
        botLeftCoord = new Vector3(xMin, yMin, 0);
        topRightCoord = new Vector3(xMax, yMax, 0);
    }

    // Function that adds a coordinate to the occupied coordinates list
    public void AddToOccupiedCoordinates(Vector3 coordinate)
    {
        occupiedCoords.Add(coordinate);
    }

    // Function that generates the background on a dungeon
    public void GenerateBackground()
    {
        // Add the spawn to occupied coordinates
        occupiedCoords.Add(new Vector3(0f, 0f, 0f));

        // Set the bottom left and top right coordinates
        int bottomBoundX = (int)botLeftCoord.x - 60;
        int bottomBoundY = (int)botLeftCoord.y - 60;
        int topBoundX = (int)topRightCoord.x + 60;
        int topBoundY = (int)topRightCoord.y + 60;

        // Instantiate background rooms in every spot, except for occupied coordinates
        for (int i = bottomBoundX; i <= topBoundX; i += 30) // X coordinates
        {
            for (int j = bottomBoundY; j <= topBoundY; j += 30) // Y coordinates
            {
                Vector3 currentCoord = new Vector3(i, j, 0f);
                if (!occupiedCoords.Contains(currentCoord))
                {
                    // Spawn a background tile there
                    Instantiate(backgroundRoom, currentCoord, transform.rotation);
                }
            }
        }
    }

    // Function that returns whether or not the background has been generated
    public bool GetBackgroundGenerated()
    {
        return backgroundGenerated;
    }
}
