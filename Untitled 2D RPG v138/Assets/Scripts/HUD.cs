using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static HUD instance;

    [Header("Clusters and Panel Sections")]
    [SerializeField] GameObject dialogueCluster;
    [SerializeField] GameObject botLeftCluster;
    [SerializeField] GameObject botRightCluster;
    [SerializeField] GameObject actionButtonsCluster;
    [SerializeField] GameObject levelUpPanel;
    [SerializeField] GameObject minimapBackground;
    [SerializeField] GameObject minimapTexture;

    [Header("Warning Panel")]
    [SerializeField] GameObject warningPanel;
    [SerializeField] Text warningText;

    [Header("Timer Panel")]
    [SerializeField] GameObject timerPanel;
    [SerializeField] Text timerText;
    [SerializeField] Text arrowCount;
    [SerializeField] Text enemyCount;
    [SerializeField] Text baronCount;

    [Header("Pause Menu")]
    [SerializeField] GameObject pauseMenu;

    [Header("Inventory Menu")]
    [SerializeField] GameObject inventoryMenu;
    [SerializeField] Text inventoryArrowCount;

    [Header("Buy Menu Items")]
    [SerializeField] GameObject buyMenu;
    [SerializeField] GameObject buyDialogue;
    [SerializeField] GameObject buyDialogueBoxSingle;
    [SerializeField] GameObject buyDialogueBoxMultiple;

    [Header("Sell Menu Items")]
    [SerializeField] GameObject sellMenu;
    [SerializeField] GameObject sellDialogue;

    [Header("Full Map Panel")]
    [SerializeField] GameObject mapPanel;

    [Header("Game Over Panel")]
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] Text gameOverSubtitle;

    [Header("Victory Panel")]
    [SerializeField] GameObject victoryPanel;
    [SerializeField] Text timeBonusLevel;
    [SerializeField] Image timeBonusImage;
    [SerializeField] Text coinBonusAmount;
    [SerializeField] Text xpBonusAmount;

    [Header("Victory Panel - Bonus Level Sprites")]
    [SerializeField] Sprite goldLevel;
    [SerializeField] Sprite silverLevel;
    [SerializeField] Sprite bronzeLevel;

    [Header("Welcome Panels")]
    [SerializeField] GameObject welcomePanel1;
    [SerializeField] GameObject welcomePanel2;
    [SerializeField] GameObject welcomePanel3;


    [Header("Abilities Bar")]
    [SerializeField] GameObject abilitiesBar;
    [SerializeField] Image cooldownImg0;
    [SerializeField] Image cooldownImg1;
    [SerializeField] Image cooldownImg2;
    [SerializeField] Image cooldownImg3;

    private bool pauseMenuOpen = false;
    private bool inventoryMenuOpen = false;
    private bool buyMenuOpen = false;
    private bool sellMenuOpen = false;
    private bool timerPanelOpen = false;
    private bool mapPanelOpen = false;
    private bool gameOverPanelOpen = false;
    private bool victoryPanelOpen = false;
    private bool welcomePanelOpen = false;

    bool gameStarted = false;

    GameObject player;
    GameObject gameController;

    // Start is called before the first frame update
    void Start()
    {
        // Keep this object in all scenes; destroy clones
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        EnableActionButtonsCluster(false);

        // Initializations
        player = GameObject.FindWithTag("Player");
        gameController = GameObject.FindWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        CheckForPauseToggle();
        CheckForInventoryToggle();
        CheckForMapToggle();

        // Set timer text constantly
        if (timerPanelOpen)
        {
            SetTimerText(GameObject.FindWithTag("DungeonManager").GetComponent<DungeonTimer>().GetTimerString());
        }

        if (player.GetComponent<PlayerController>() != null && gameController.GetComponent<GameController>() != null && !gameStarted)
        {
            // Open the welcome panel on start
            OpenWelcomePanel();

            // Game has started
            gameStarted = true;
        }
    }

    // PRIVATE FUNCTIONS

    // Function that checks constantly to see if the player has opened the pause menu
    private void CheckForPauseToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !pauseMenuOpen)
        {
            if (gameController.GetComponent<GameController>().GetCanOpenMenus())
            {
                OpenPauseMenu();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && pauseMenuOpen)
        {
            ClosePauseMenu();
        }
    }

    // Function that checks constantly to see if the player has opened the inventory menu
    private void CheckForInventoryToggle()
    {
        if (Input.GetKeyDown(KeyCode.I) && !inventoryMenuOpen)
        {
            if (gameController.GetComponent<GameController>().GetCanOpenMenus() && gameController.GetComponent<GameController>().GetCanOpenInventory())
            {
                OpenInventoryMenu();
            }
        }
        else if (Input.GetKeyDown(KeyCode.I) && inventoryMenuOpen)
        {
            CloseInventoryMenu();
        }
    }

    // Function that checks constantly to see if the player has opened the map menu
    private void CheckForMapToggle()
    {
        if (Input.GetKeyDown(KeyCode.M) && !mapPanelOpen)
        {
            if (player.GetComponent<PlayerController>().GetInDungeon()) // Open the map if the player is in a dungeon
            {
                if (gameController.GetComponent<GameController>().GetCanOpenMenus())
                {
                    OpenMapPanel();
                }
            }
            else // If not in a dungeon, display a warning that the player is not in a dungeon.
            {
                EnableWarningPanel(true);
                SetWarningText("The map is only available in dungeons!");
            }
        }
        else if (Input.GetKeyDown(KeyCode.M) && mapPanelOpen)
        {
            CloseMapPanel();
        }
    }

    // PUBLIC FUNCTIONS

    // Function that toggles the bottom left cluster
    public void EnableBotLeftCluster(bool value)
    {
        botLeftCluster.SetActive(value);
    }

    // Function that toggles the bottom right cluster
    public void EnableBotRightCluster(bool value)
    {
        botRightCluster.SetActive(value);
    }

    // Function that toggles the dialogue cluster
    public void EnableDialogueCluster(bool value)
    {
        dialogueCluster.SetActive(value);
    }

    // Function that toggles the action buttons cluster
    public void EnableActionButtonsCluster(bool value)
    {
        actionButtonsCluster.SetActive(value);
    }

    // Function that toggles the level up panel
    public void EnableLevelUpPanel(bool value)
    {
        levelUpPanel.SetActive(value);
    }

    // Function that toggles the warning panel
    public void EnableWarningPanel(bool value)
    {
        warningPanel.SetActive(value);
    }

    // Function that sets the warning panel text
    public void SetWarningText(string theText)
    {
        warningText.text = theText;
    }

    // Function that toggles the timer panel
    public void EnableTimerPanel(bool value)
    {
        if (value == true)
        {
            timerPanelOpen = true;
        }
        else
        {
            timerPanelOpen = false;
        }

        timerPanel.SetActive(value);
    }

    // Function that sets the timer panel text
    public void SetTimerText(string theText)
    {
        timerText.text = theText;
    }

    public bool AnyMenuOpen()
    {
        if (pauseMenuOpen || inventoryMenuOpen || mapPanelOpen || buyMenuOpen
            || sellMenuOpen || gameOverPanelOpen || victoryPanelOpen || welcomePanelOpen)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool AnyMenuOpen_NotIncludingVictory()
    {
        if (pauseMenuOpen || inventoryMenuOpen || mapPanelOpen || buyMenuOpen
            || sellMenuOpen || gameOverPanelOpen || welcomePanelOpen)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CloseAllMenusExcept(GameObject menu)
    {
        // Close any menu that is not the given menu
        if (menu != pauseMenu) { ClosePauseMenu(); }
        if (menu != inventoryMenu) { CloseInventoryMenu(); }
        if (menu != buyMenu) { CloseBuyMenu(); }
        if (menu != sellMenu) { CloseSellMenu(); }
        if (menu != mapPanel) { CloseMapPanel(); }
        if (menu != gameOverPanel) { CloseGameOverPanel(); }
        if (menu != victoryPanel) { CloseVictoryPanel(); }
        if (menu != welcomePanel1) { CloseWelcomePanels(); }
    }

    // Function that opens the pause menu
    public void OpenPauseMenu()
    {
        // Close the warning if it is open
        EnableWarningPanel(false);

        pauseMenu.SetActive(true);
        gameController.GetComponent<GameController>().FreezeMovingObjects();
        pauseMenuOpen = true;

        // Cannot open any other menus
        gameController.GetComponent<GameController>().SetCanOpenMenus(false);
    }

    // Function that closes the pause menu
    public void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);
        gameController.GetComponent<GameController>().UnfreezeMovingObjects();
        pauseMenuOpen = false;

        // Can open menus again
        gameController.GetComponent<GameController>().SetCanOpenMenus(true);
    }

    // Function that opens the inventory menu
    public void OpenInventoryMenu()
    {
        // Close the warning if it is open
        EnableWarningPanel(false);

        inventoryMenu.SetActive(true);
        gameController.GetComponent<GameController>().FreezeMovingObjects();
        player.GetComponent<PlayerAbilities>().FreezeAbilities();
        inventoryMenuOpen = true;

        // Refresh the arrow count text
        SetInventoryArrowCountText(player.GetComponent<ArrowCount>().GetArrowCount());

        // Cannot open any other menus
        gameController.GetComponent<GameController>().SetCanOpenMenus(false);
    }

    // Function that closes the inventory menu
    public void CloseInventoryMenu()
    {
        inventoryMenu.SetActive(false);
        gameController.GetComponent<GameController>().UnfreezeMovingObjects();
        player.GetComponent<PlayerAbilities>().UnfreezeAbilities();
        inventoryMenuOpen = false;

        // Can open menus again
        gameController.GetComponent<GameController>().SetCanOpenMenus(true);
    }

    // Function that opens the map panel
    public void OpenMapPanel()
    {
        // Close the warning if it is open
        EnableWarningPanel(false);

        mapPanel.SetActive(true);
        gameController.GetComponent<GameController>().FreezeMovingObjects();
        player.GetComponent<PlayerAbilities>().FreezeAbilities();
        mapPanelOpen = true;

        // Cannot open any other menus
        gameController.GetComponent<GameController>().SetCanOpenMenus(false);
    }

    // Function that closes the map panel
    public void CloseMapPanel()
    {
        mapPanel.SetActive(false);
        gameController.GetComponent<GameController>().UnfreezeMovingObjects();
        player.GetComponent<PlayerAbilities>().UnfreezeAbilities();
        mapPanelOpen = false;

        // Can open menus again
        gameController.GetComponent<GameController>().SetCanOpenMenus(true);
    }

    // Function that opens the game over panel
    public void OpenGameOverPanel(string deathReason)
    {
        // Close the warning if it is open
        EnableWarningPanel(false);

        // Set the subtitle text properly
        gameOverSubtitle.text = deathReason;

        gameOverPanel.SetActive(true);
        gameController.GetComponent<GameController>().FreezeMovingObjects();
        player.GetComponent<PlayerAbilities>().FreezeAbilities();
        gameOverPanelOpen = true;

        // Cannot open any other menus
        gameController.GetComponent<GameController>().SetCanOpenMenus(false);
    }

    // Function that closes the game over panel
    public void CloseGameOverPanel()
    {
        gameOverPanel.SetActive(false);
        gameController.GetComponent<GameController>().UnfreezeMovingObjects();
        player.GetComponent<PlayerAbilities>().UnfreezeAbilities();
        gameOverPanelOpen = false;

        // Can open menus again
        gameController.GetComponent<GameController>().SetCanOpenMenus(true);
    }

    // Function that opens the victory panel
    public void OpenVictoryPanel()
    {
        // Close the warning if it is open
        EnableWarningPanel(false);

        victoryPanel.SetActive(true);
        victoryPanelOpen = true;

        // Close every other menu
        CloseAllMenusExcept(victoryPanel);

        // Cannot open any other menus
        gameController.GetComponent<GameController>().SetCanOpenMenus(false);
    }

    // Function that closes the victory panel
    public void CloseVictoryPanel()
    {
        victoryPanel.SetActive(false);
        victoryPanelOpen = false;

        // Can open menus again
        gameController.GetComponent<GameController>().SetCanOpenMenus(true);
    }

    // Function that opens the welcome panel
    public void OpenWelcomePanel()
    {
        welcomePanel1.SetActive(true);
        gameController.GetComponent<GameController>().FreezeMovingObjects();
        player.GetComponent<PlayerAbilities>().FreezeAbilities();
        welcomePanelOpen = true;

        // Cannot open any other menus
        gameController.GetComponent<GameController>().SetCanOpenMenus(false);
    }

    // Function that closes all the welcome panel
    public void CloseWelcomePanels()
    {
        welcomePanel1.SetActive(false);
        welcomePanel2.SetActive(false);
        welcomePanel3.SetActive(false);

        gameController.GetComponent<GameController>().UnfreezeMovingObjects();
        player.GetComponent<PlayerAbilities>().UnfreezeAbilities();
        welcomePanelOpen = false;

        // Can open menus again
        gameController.GetComponent<GameController>().SetCanOpenMenus(true);
    }

    // Function that opens the buy menu
    public void OpenBuyMenu()
    {
        // Close the warning if it is open
        EnableWarningPanel(false);

        // Close the sell menu
        CloseSellMenu();

        // Since nothing is selected, make the sprite blank
        GetComponent<StoreController>().ResetBuyMenuImage();

        buyMenu.SetActive(true);
        gameController.GetComponent<GameController>().FreezeMovingObjects();
        player.GetComponent<PlayerAbilities>().FreezeAbilities();
        buyMenuOpen = true;

        // Cannot open any other menus
        gameController.GetComponent<GameController>().SetCanOpenMenus(false);
    }

    // Function that closes the buy menu
    public void CloseBuyMenu()
    {
        buyMenu.SetActive(false);
        gameController.GetComponent<GameController>().UnfreezeMovingObjects();
        player.GetComponent<PlayerAbilities>().UnfreezeAbilities();
        buyMenuOpen = false;

        // Indicate that the player is done talking to a shopkeeper
        player.GetComponent<PlayerController>().TalkingToShopKeeper(false);

        // Deactivate all the checkmarks
        GetComponent<StoreController>().DeactivateAllCheckmarks();

        // Can open menus again
        gameController.GetComponent<GameController>().SetCanOpenMenus(true);
    }

    // Function that opens the Buy Dialogue widget
    public void OpenBuyDialogue()
    {
        // Close the warning if it is open
        EnableWarningPanel(false);

        // Only open the buy dialogue if an item is selected
        if (GetComponent<StoreController>().IsItemSelected("buy"))
        {
            // Reset the quantity text
            GetComponent<StoreController>().ResetBuyQuantity();

            // Open the widget
            buyDialogue.SetActive(true);

            if (GetComponent<StoreController>().GetSelectedWeaponName() == "Arrows")  // Quantity selection scenario
            {
                // Turn off "Single" and turn on "Multiple"
                ToggleBuyDialogueBoxSingle(false);
                ToggleBuyDialogueBoxMultiple(true);

                // Set the "In Inventory" panel
                GetComponent<StoreController>().SetInInventoryBox("arrow");

                // Set the text
                GetComponent<StoreController>().SetBuyDialogueText_Multiple(GetComponent<StoreController>().GetSelectedWeaponName() + "? How many would you like to buy?");
            }
            else  // Non-quantity selection scenario
            {
                // Turn off "Multiple" and turn on "Single"
                ToggleBuyDialogueBoxSingle(true);
                ToggleBuyDialogueBoxMultiple(false);

                // Set the text
                GetComponent<StoreController>().SetBuyDialogueText(GetComponent<StoreController>().GetSelectedWeaponName() + "? That will be $" + GetComponent<StoreController>().GetSelectedBuyValue() + ". Is that okay?");
            }
        }
    }

    // Function that opens the Buy Dialogue widget
    public void OpenSellDialogue()
    {
        // Close the warning if it is open
        EnableWarningPanel(false);

        // Only open the buy dialogue if an item is selected
        if (GetComponent<StoreController>().IsItemSelected("sell"))
        {
            // Open the widget
            sellDialogue.SetActive(true);

            // Set the proper text

            // Only one item
            if (GetComponent<StoreController>().GetSelectedItems().Count == 1)
            {
                GetComponent<StoreController>().SetSellDialogueText(
                    GetComponent<StoreController>().GetSelectedItems()[0].GetWeaponName() + "? I can give you $" + GetComponent<StoreController>().GetSelectedItems()[0].GetSellValue().ToString() + " for that. Is that okay?"
                );
            }
            else // Multiple items
            {
                GetComponent<StoreController>().SetSellDialogueText(
                    "I can give you $" + GetComponent<StoreController>().GetSelectedItemsValue().ToString() + " for those. Is that okay?"
                );
            }
        }
    }

    // Function that closes the Buy Dialogue widget
    public void CloseBuyDialogue()
    {
        buyDialogue.SetActive(false);
    }

    // Function that closes the sell Dialogue widget
    public void CloseSellDialogue()
    {
        sellDialogue.SetActive(false);
    }

    // Function that opens the buy menu
    public void OpenSellMenu()
    {
        // Close the warning if it is open
        EnableWarningPanel(false);

        // Since nothing is selected, make the sprite blank
        GetComponent<StoreController>().ResetSellMenuImage();

        // Transfer the player's inventory to the sell menu
        GetComponent<StoreController>().SetSellMenu();

        // Close the buy menu
        CloseBuyMenu();

        sellMenu.SetActive(true);
        gameController.GetComponent<GameController>().FreezeMovingObjects();
        player.GetComponent<PlayerAbilities>().FreezeAbilities();
        sellMenuOpen = true;

        // Cannot open any other menus
        gameController.GetComponent<GameController>().SetCanOpenMenus(false);
    }

    // Function that closes the buy menu
    public void CloseSellMenu()
    {
        // Deactivate all checkmarks
        GetComponent<StoreController>().DeactivateAllCheckmarks();

        // Reset the total sell value of items selected
        GetComponent<StoreController>().ResetTotalSellValue();

        sellMenu.SetActive(false);
        gameController.GetComponent<GameController>().UnfreezeMovingObjects();
        player.GetComponent<PlayerAbilities>().UnfreezeAbilities();
        sellMenuOpen = false;

        // Reactivate every image
        GetComponent<StoreController>().ReactivateSellStoreImages();

        // Remove every item from the list of selected items
        GetComponent<StoreController>().RemoveAllSelectedItems();

        // Can open menus again
        gameController.GetComponent<GameController>().SetCanOpenMenus(true);
    }

    // Function that toggles the minimap background
    public void EnableMinimapBackground(bool value)
    {
        minimapBackground.SetActive(value);
    }

    // Function that toggles the abilities bar
    public void EnableAbilitiesBar(bool value)
    {
        abilitiesBar.SetActive(value);
    }

    // Function that toggles cooldown images for abilities
    public void EnableCooldownImage(int imageNum, bool value)
    {
        if (imageNum == 0)
        {
            cooldownImg0.gameObject.SetActive(value);
        }
        else if (imageNum == 1)
        {
            cooldownImg1.gameObject.SetActive(value);
        }
        else if (imageNum == 2)
        {
            cooldownImg2.gameObject.SetActive(value);
        }
        else if (imageNum == 3)
        {
            cooldownImg3.gameObject.SetActive(value);
        }
    }

    // Function that changes the fill amount for the cooldown images
    public void SetCooldownImageFill(int imageNum, float amount)
    {
        if (imageNum == 0)
        {
            cooldownImg0.fillAmount = amount;
        }
        else if (imageNum == 1)
        {
            cooldownImg1.fillAmount = amount;
        }
        else if (imageNum == 2)
        {
            cooldownImg2.fillAmount = amount;
        }
        else if (imageNum == 3)
        {
            cooldownImg3.fillAmount = amount;
        }
    }

    // Function that sets the arrow count text
    public void SetArrowCountText(int amount)
    {
        if (amount < 10)
        {
            arrowCount.text = "x0" + amount.ToString();
        }
        else
        {
            arrowCount.text = "x" + amount.ToString();
        }
    }

    public void SetEnemyCountText(int amount)
    {
        // If the player is dead, enemy count should be nullified
        if (player.GetComponent<Health>().GetPlayerDead())
        {
            enemyCount.text = "x--";
        }
        else if (amount < 10) // Less than ten, add a "0"
        {
            enemyCount.text = "x0" + amount.ToString();
        }
        else // Ten or more
        {
            enemyCount.text = "x" + amount.ToString();
        }
    }

    public void SetBaronCountText(int amount)
    {
        // If the player is dead, enemy count should be nullified
        if (player.GetComponent<Health>().GetPlayerDead())
        {
            baronCount.text = "x--";
        }
        else if (amount < 10) // Less than ten, add a "0"
        {
            baronCount.text = "x0" + amount.ToString();
        }
        else // Ten or more
        {
            baronCount.text = "x" + amount.ToString();
        }
    }

    // Function for pressing the Pause Menu's "Controls" button
    public void ControlsButton()
    {
        // Close the pause menu
        ClosePauseMenu();

        // Open the welcome panel
        OpenWelcomePanel();
    }

    // Function for quitting the game
    public void QuitGame()
    {
        Application.Quit();
    }

    // Function for toggling the Buy Dialogue Box Single
    public void ToggleBuyDialogueBoxSingle(bool value)
    {
        buyDialogueBoxSingle.SetActive(value);
    }

    // Function for toggling the Buy Dialogue Box Multiple
    public void ToggleBuyDialogueBoxMultiple(bool value)
    {
        buyDialogueBoxMultiple.SetActive(value);
    }

    // Function for setting the inventory arrow count text
    public void SetInventoryArrowCountText(int count)
    {
        if (count < 10)
        {
            inventoryArrowCount.text = "x0" + count.ToString();
        }
        else
        {
            inventoryArrowCount.text = "x" + count.ToString();
        }
    }

    // Function for setting the time bonus level text
    public void SetTimeBonusLevelText(string theText)
    {
        timeBonusLevel.text = theText;
    }

    // Function for setting the coin bonus amount text
    public void SetCoinBonusAmountText(string theText)
    {
        coinBonusAmount.text = theText;
    }

    // Function for setting the XP bonus amount text
    public void SetXPBonusAmountText(string theText)
    {
        xpBonusAmount.text = theText;
    }

    // Function that sets the time bonus image sprite
    public void SetTimeBonusImageSprite(string level)
    {
        if (level == "gold")
        {
            timeBonusImage.sprite = goldLevel;
        }
        else if (level == "silver")
        {
            timeBonusImage.sprite = silverLevel;
        }
        else if (level == "bronze")
        {
            timeBonusImage.sprite = bronzeLevel;
        }
    }

    // Function for getting whether or not the victory panel is open
    // (We specifically want to check this one for "post-dungeon combat" purposes)
    public bool GetVictoryPanelOpen()
    {
        return victoryPanelOpen;
    }

    // Function for toggling the minimap texture
    public void ToggleMinimapTexture(bool value)
    {
        minimapTexture.SetActive(value);
    }
}