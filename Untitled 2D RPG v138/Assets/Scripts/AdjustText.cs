using UnityEngine;
using System.Collections;

public class AdjustText : MonoBehaviour
{
    // Simple script for moving the TextMesh to the top layer
    public string layerToPushTo;

    void Start()
    {
        GetComponent<Renderer>().sortingLayerName = layerToPushTo;
    }
}
