using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedDot : MonoBehaviour
{
    [Header("Active Time")]
    [SerializeField] float activeTime;

    private void Update()
    {
        activeTime -= Time.deltaTime;
        if (activeTime <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
