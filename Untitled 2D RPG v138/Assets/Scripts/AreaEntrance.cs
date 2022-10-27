using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEntrance : MonoBehaviour
{
    [SerializeField] string entryGate;
    [SerializeField] string playerDirection;

    GameObject player;
    GameObject gameController;
    GameObject hud;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        hud = GameObject.FindWithTag("HUD");
        gameController = GameObject.FindWithTag("GameController");

        // Enable the minimap background if there's a minimap
        if (GameObject.FindWithTag("MinimapCamera"))
        {
            // "Do it here"
            hud.GetComponent<HUD>().EnableMinimapBackground(true);
            hud.GetComponent<HUD>().ToggleMinimapTexture(true);
        }
        else
        {
            hud.GetComponent<HUD>().EnableMinimapBackground(false);
            hud.GetComponent<HUD>().ToggleMinimapTexture(false);
        }
    }

    // PUBLIC FUNCTIONS

    // Function for setting the player's position to that of this gate
    public void EnterPlayer()
    {
        // Unfreeze the player
        gameController.GetComponent<GameController>().UnfreezeMovingObjects();
        player.GetComponent<PlayerAbilities>().UnfreezeAbilities();

        player.transform.position = gameObject.transform.position;

        // Set the desired player direction upon entry
        if (playerDirection == "up")
        {
            player.GetComponent<Animator>().SetFloat("lastMoveY", 1f);
            player.GetComponent<Animator>().SetFloat("lastMoveX", 0f);
            player.GetComponent<Animator>().SetFloat("moveX", 0);
            player.GetComponent<Animator>().SetFloat("moveY", 0);
        }
        else if (playerDirection == "down")
        {
            player.GetComponent<Animator>().SetFloat("lastMoveY", -1f);
            player.GetComponent<Animator>().SetFloat("lastMoveX", 0f);
            player.GetComponent<Animator>().SetFloat("moveX", 0);
            player.GetComponent<Animator>().SetFloat("moveY", 0);
        }
        else if (playerDirection == "left")
        {
            player.GetComponent<Animator>().SetFloat("lastMoveX", -1f);
            player.GetComponent<Animator>().SetFloat("lastMoveY", 0f);
            player.GetComponent<Animator>().SetFloat("moveX", 0);
            player.GetComponent<Animator>().SetFloat("moveY", 0);
        }
        else if (playerDirection == "right")
        {
            player.GetComponent<Animator>().SetFloat("lastMoveX", 1f);
            player.GetComponent<Animator>().SetFloat("lastMoveY", 0f);
            player.GetComponent<Animator>().SetFloat("moveX", 0);
            player.GetComponent<Animator>().SetFloat("moveY", 0);
        }
    }

    // Function for getting the entry gate name
    public string GetEntryGate()
    {
        return entryGate;
    }
}
