using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInSeconds : MonoBehaviour
{
    [SerializeField] private float secondsTillDestruction = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, secondsTillDestruction);
    }
}
