using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] float interactionRange = 1.0f;

    [SerializeField] List<string> dialogueLines;
    // 0 = Regular dialogue; 1 = Question; 2 = 'Yes' answer; 3 = 'No' answer
    [SerializeField] List<int> questionIndicationList;

    // 0 = Not an 'end case'; 1 = Is an 'end case'
    // Where an 'end case' is a dialogue line that should be the last.
    [SerializeField] List<int> endCases;

    [SerializeField] Sprite npcFace;
    [SerializeField] string npcName;

    [Header("Interaction Sprite Details")]
    [SerializeField] GameObject interactionSprite;
    [SerializeField] float showSpriteAfter = 0.25f;
    [SerializeField] float hideSpriteAfter = 0.25f;
    bool showIntSprite = false;
    float showSpriteAfterCounter;
    float hideSpriteAfterCounter;

    [SerializeField] bool isShopkeeper;

    GameObject player;
    Animator animator;
    GameObject hud;
    GameObject gameController;

    float counterDistanceMin = 2.60f;
    float counterDistanceMax = 2.85f;

    float xShopMin = -0.8f;
    float xShopMax = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        hud = GameObject.FindWithTag("HUD");
        animator = GetComponent<Animator>();
        gameController = GameObject.FindWithTag("GameController");

        // Initialize the counters
        showSpriteAfterCounter = showSpriteAfter;
        hideSpriteAfterCounter = hideSpriteAfter;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForInteraction();
        InteractionSpriteCountdown();
    }

    // PRIVATE FUNCTIONS

    // Function that monitors and sees if the player is interacting with the NPC or not
    private void CheckForInteraction()
    {
        float distanceBetween = Vector2.Distance(player.transform.position, transform.position);

        if (distanceBetween <= interactionRange)
        {
            // Enable the interaction sprite (unless the player is in dialogue, then disable it)
            if (!player.GetComponent<PlayerController>().GetInDialogue()) { ToggleInteractionSprite(true); }
            else { ToggleInteractionSprite(false); }
            
            if (Input.GetKeyDown(KeyCode.E) && !hud.GetComponent<HUD>().AnyMenuOpen()) // Can't re-initiate dialogue when a menu is opened
            {
                // Go through the steps to start dialogue
                InitiateDialogue();

                // Pause the NPC's wandering
                if (GetComponent<Wander>() != null) { GetComponent<Wander>().ToggleNPCWandering(false); }
            }
        }
        else
        {
            // Out of range; disable interaction sprite
            ToggleInteractionSprite(false);
        }

        if (isShopkeeper && distanceBetween > counterDistanceMin && distanceBetween < counterDistanceMax && transform.position.y > player.transform.position.y && player.transform.position.x > xShopMin && player.transform.position.x < xShopMax)
        {
            // Enable the interaction sprite (unless the player is in dialogue, then disable it)
            if (!player.GetComponent<PlayerController>().GetInDialogue()) { ToggleInteractionSprite(true); }
            else { ToggleInteractionSprite(false); }

            if (Input.GetKeyDown(KeyCode.E) && !hud.GetComponent<HUD>().AnyMenuOpen()) // Can't re-initiate dialogue when a menu is opened
            {
                // Go through the steps to start dialogue
                InitiateDialogue();

                // Pause the NPC's wandering
                if (GetComponent<Wander>() != null) { GetComponent<Wander>().ToggleNPCWandering(false); }
            }
        }
        else if (isShopkeeper && distanceBetween > interactionRange)
        {
            // Is a shopkeeper and not in normal range either, disable the interaction sprite
            ToggleInteractionSprite(false);
        }
    }

    // Function that counts down, then shows the interaction sprite (for the purpose of smoothing its appearance over)
    private void InteractionSpriteCountdown()
    {
        if (showIntSprite) // When true, countdown to showing sprite
        {
            // Re-initialize 'hide' counter
            hideSpriteAfterCounter = hideSpriteAfter;

            showSpriteAfterCounter -= Time.deltaTime;
            if (showSpriteAfterCounter <= 0f)
            {
                interactionSprite.SetActive(true);
            }
        }
        else // When false, countdown to hiding sprite
        {
            // Re-initialize 'show' counter
            showSpriteAfterCounter = showSpriteAfter;

            // If we're in dialogue, hide immediately
            if (player.GetComponent<PlayerController>().GetInDialogue())
            {
                hideSpriteAfterCounter = 0f;
            }

            hideSpriteAfterCounter -= Time.deltaTime;
            if (hideSpriteAfterCounter <= 0f)
            {
                interactionSprite.SetActive(false);
            }
        }
    }

    // Function that sets up the conditions for a dialogue
    private void InitiateDialogue()
    {
        // Close the warning menu if it is open
        hud.GetComponent<HUD>().EnableWarningPanel(false);

        // Close the abilities bar
        hud.GetComponent<HUD>().EnableAbilitiesBar(false);

        // Face the player
        ChangeDirection();

        // Send dialogue lines over the HUD, and the sprite for the NPC's face, and the NPC's name
        hud.GetComponent<Dialogue>().SetDialogue(dialogueLines, questionIndicationList, endCases);
        hud.GetComponent<Dialogue>().SetDialogueFace(npcFace);
        hud.GetComponent<Dialogue>().SetNPCName(npcName);

        // Trigger dialogue
        hud.GetComponent<HUD>().EnableBotLeftCluster(false);
        hud.GetComponent<HUD>().EnableBotRightCluster(false);
        hud.GetComponent<HUD>().EnableLevelUpPanel(false);
        hud.GetComponent<HUD>().EnableDialogueCluster(true);
        player.GetComponent<PlayerController>().SetInDialogue(true);

        // Freeze all moving objects
        gameController.GetComponent<GameController>().FreezeMovingObjects();
        player.GetComponent<PlayerAbilities>().FreezeAbilities();

        // Cannot open the inventory menu while in dialogue
        gameController.GetComponent<GameController>().SetCanOpenInventory(false);

        // If this is a shopkeeper, set the relevant boolean on the player to "true"
        if (isShopkeeper)
        {
            player.GetComponent<PlayerController>().TalkingToShopKeeper(true);
        }
    }

    // Function that changes the NPC's direction
    private void ChangeDirection()
    {
        float differenceInX = Mathf.Abs(player.transform.position.x - transform.position.x);
        float differenceInY = Mathf.Abs(player.transform.position.y - transform.position.y);

        bool xPriority = false;
        bool yPriority = false;

        if (differenceInX >= differenceInY)
        {
            xPriority = true;
        }
        else
        {
            yPriority = true;
        }

        // Favor the X axis
        if (xPriority)
        {
            // Face Left
            if (player.transform.position.x < transform.position.x)
            {
                animator.SetFloat("directionX", -1);
                animator.SetFloat("directionY", 0);

                // Have the player face right
                player.GetComponent<Animator>().SetFloat("lastMoveX", 1f);
                player.GetComponent<Animator>().SetFloat("lastMoveY", 0f);
                player.GetComponent<Animator>().SetFloat("moveX", 0);
                player.GetComponent<Animator>().SetFloat("moveY", 0);
            }
            // Face Right
            else if (player.transform.position.x > transform.position.x)
            {
                animator.SetFloat("directionX", 1);
                animator.SetFloat("directionY", 0);

                // Have the player face left
                player.GetComponent<Animator>().SetFloat("lastMoveX", -1f);
                player.GetComponent<Animator>().SetFloat("lastMoveY", 0f);
                player.GetComponent<Animator>().SetFloat("moveX", 0);
                player.GetComponent<Animator>().SetFloat("moveY", 0);
            }
        }

        else if (yPriority)
        {
            // Face Up
            if (player.transform.position.y > transform.position.y)
            {
                animator.SetFloat("directionY", 1);
                animator.SetFloat("directionX", 0);

                // Have the player face down
                player.GetComponent<Animator>().SetFloat("lastMoveY", -1f);
                player.GetComponent<Animator>().SetFloat("lastMoveX", 0f);
                player.GetComponent<Animator>().SetFloat("moveX", 0);
                player.GetComponent<Animator>().SetFloat("moveY", 0);
            }
            // Face Right
            else if (player.transform.position.y < transform.position.y)
            {
                animator.SetFloat("directionY", -1);
                animator.SetFloat("directionX", 0);

                // Have the player face up
                player.GetComponent<Animator>().SetFloat("lastMoveY", 1f);
                player.GetComponent<Animator>().SetFloat("lastMoveX", 0f);
                player.GetComponent<Animator>().SetFloat("moveX", 0);
                player.GetComponent<Animator>().SetFloat("moveY", 0);
            }
        }
    }

    // Function that toggles the interaction sprite
    private void ToggleInteractionSprite(bool value)
    {
        showIntSprite = value;
    }
}
