using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaExit : MonoBehaviour
{
    [SerializeField] string areaToLoad;
    [SerializeField] string entryGate;
    [SerializeField] float loadTime = 1f;

    GameObject master;
    GameObject gameController;
    GameObject player;
    GameObject hud;

    bool startLoading;

    // Start is called before the first frame update
    void Start()
    {
        master = GameObject.FindWithTag("Master");
        gameController = GameObject.FindWithTag("GameController");
        player = GameObject.FindWithTag("Player");
        hud = GameObject.FindWithTag("HUD");
    }

    // Update is called once per frame
    void Update()
    {
        if (startLoading)
        {
            // Fade to black
            hud.GetComponent<UIFade>().FadeToBlack();

            // Prevent the player from moving
            gameController.GetComponent<GameController>().FreezeMovingObjects();
            player.GetComponent<PlayerAbilities>().FreezeAbilities();

            // Countdown loading
            loadTime -= Time.deltaTime;

            // Load time is up
            if (loadTime <= 0f)
            {
                // Load the scene when the time is right
                SceneManager.LoadScene(areaToLoad);
            }
        }
    }

    // Function for when the player enters the transition zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Exit has been triggered
        if (other.CompareTag("Player"))
        {
            // Set the entry gate on the master
            master.GetComponent<Master>().SetEntryGate(entryGate);

            // Begin the countdown
            startLoading = true;
        }
    }
}
