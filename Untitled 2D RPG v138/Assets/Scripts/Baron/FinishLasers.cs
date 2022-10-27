using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLasers : MonoBehaviour
{
    // Function called in animation when laser-firing is done
    public void _FinishLasers()
    {
        GetComponentInParent<BaronAnimations>().DeactivateLasers();
    }
}
