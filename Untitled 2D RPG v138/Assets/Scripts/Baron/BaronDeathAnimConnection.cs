using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaronDeathAnimConnection : MonoBehaviour
{
    // Connects to the Health script in child
    public void CallDeathAnimation()
    {
        GetComponentInChildren<Health>().DeathAnimation("die");
    }
}
