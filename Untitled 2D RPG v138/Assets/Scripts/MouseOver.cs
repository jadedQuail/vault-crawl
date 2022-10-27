using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOver : MonoBehaviour
{
    bool overObject = false;

    GameObject player;
    GameObject hud;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        hud = GameObject.FindWithTag("HUD");
    }

    private void Update()
    {
        if (IsPointerOverUIObject())
        {
            overObject = true;
            player.GetComponent<PlayerController>().SetCanAttack(false);
        }
        else
        {
            overObject = false;
            player.GetComponent<PlayerController>().SetCanAttack(true);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            // If the player is over the object and clicks, disable it and let
            // them attack again
            if (overObject)
            {
                hud.GetComponent<HUD>().EnableLevelUpPanel(false);
                player.GetComponent<PlayerController>().SetCanAttack(true);
                overObject = false;
            }
        }
    }

    // Function that detects whether or not the mouse is hovering over this object
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
