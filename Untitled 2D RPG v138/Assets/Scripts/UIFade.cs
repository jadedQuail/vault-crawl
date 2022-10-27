using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    [SerializeField] Image fadeScreen;
    [SerializeField] float fadeSpeed;

    bool fadeTo;
    bool fadeFrom;
    bool fadeHold;  // Stay black

    // Key gameobject references
    GameObject player;
    GameObject gameController;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize key gameobject references
        player = GameObject.FindWithTag("Player");
        gameController = GameObject.FindWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeHold)
        {
            // Screen stays black
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 1.0f);

            // Freeze the game
            gameController.GetComponent<GameController>().FreezeMovingObjects();
            player.GetComponent<PlayerAbilities>().FreezeAbilities();
            gameController.GetComponent<GameController>().SetCanOpenMenus(false);
        }

        if (fadeTo) // Fade to black
        {
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b,
                                        Mathf.MoveTowards(fadeScreen.color.a, 1f, fadeSpeed * Time.deltaTime));

            // Fade to black complete; no longer need to "fade to"
            if (fadeScreen.color.a == 1f)
            {
                fadeTo = false;
            }
        }

        if (fadeFrom) // Fade from black
        {
            // As soon as this starts, unfreeze the player and game
            gameController.GetComponent<GameController>().UnfreezeMovingObjects();
            player.GetComponent<PlayerAbilities>().UnfreezeAbilities();
            gameController.GetComponent<GameController>().SetCanOpenMenus(true);

            // Fade back in from black
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b,
                                        Mathf.MoveTowards(fadeScreen.color.a, 0f, fadeSpeed * Time.deltaTime));

            // Fade from black complete; no longer need to "fade from"
            if (fadeScreen.color.a == 0f)
            {
                fadeFrom = false;
            }
        }
    }

    // Function that triggers fading to black
    public void FadeToBlack()
    {
        fadeTo = true;
        fadeFrom = false;
        fadeHold = false;
    }

    // Function that triggers fading from black
    public void FadeFromBlack()
    {
        fadeFrom = true;
        fadeTo = false;
        fadeHold = false;
    }

    // Function that triggers the black to hold
    public void FadeHoldBlack()
    {
        fadeHold = true;
        fadeFrom = false;
        fadeTo = false;
    }

    // Function that gets whether or not the screen is currently black
    public bool GetFadeHold()
    {
        return fadeHold;
    }

    // Function that sets the fadescreen to black immediately
    public void MakeFadeScreenBlack()
    {
        fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 1f);
    }
}
