using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningFade : MonoBehaviour
{
    GameObject hud;
    private void Start()
    {
        // Initialize the HUD
        hud = GameObject.FindWithTag("HUD");
    }

    // Function called on the fade animation for the warning label
    public void EndFade()
    {
        hud.GetComponent<HUD>().EnableWarningPanel(false);
    }
}
