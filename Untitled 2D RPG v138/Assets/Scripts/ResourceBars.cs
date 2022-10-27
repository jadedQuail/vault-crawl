using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBars : MonoBehaviour
{
    // HEALTH
    [SerializeField] Image healthBackground;
    [SerializeField] Image healthForeground;

    [SerializeField] Text currentHealthText;
    [SerializeField] Text maxHealthText;

    private float playerCurrentHealth;
    private float playerMaxHealth;
    private float healthRatio;

    // MANA
    [SerializeField] Image manaBackground;
    [SerializeField] Image manaForeground;

    [SerializeField] Text currentManaText;
    [SerializeField] Text maxManaText;

    private float playerCurrentMana;
    private float playerMaxMana;
    private float manaRatio;

    // XP
    [SerializeField] Image xpBackground;
    [SerializeField] Image xpForeground;

    [SerializeField] Text levelText;

    GameObject player;

    private int playerNetXP;
    private int playerNextLevelXP;
    private int playerCurrentLevelXP;

    private float xpRatio;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        SetHealthBar();
        SetHealthTexts();
        SetManaBar();
        SetManaTexts();
        SetXPBar();
        SetXPText();
    }

    private void SetHealthBar()
    {
        (playerCurrentHealth, playerMaxHealth) = player.GetComponent<Health>().GetHealthInfo();

        healthRatio = (float)playerCurrentHealth / (float)playerMaxHealth;

        healthForeground.fillAmount = healthRatio;

        if (healthRatio <= 0)
        {
            healthForeground.enabled = false;
        }
        else
        {
            healthForeground.enabled = true;
        }
    }

    private void SetManaBar()
    {
        (playerCurrentMana, playerMaxMana) = player.GetComponent<Mana>().GetManaInfo();

        manaRatio = (float)playerCurrentMana / (float)playerMaxMana;

        manaForeground.fillAmount = manaRatio;

        if (manaRatio <= 0)
        {
            manaForeground.enabled = false;
        }
        else
        {
            manaForeground.enabled = true;
        }
    }

    private void SetXPBar()
    {
        (playerNetXP, playerNextLevelXP, playerCurrentLevelXP) = player.GetComponent<PlayerLeveling>().GetXPInfo();

        xpRatio = (float)(playerNetXP - playerCurrentLevelXP) / (playerNextLevelXP - playerCurrentLevelXP);

        xpForeground.fillAmount = xpRatio;

        if (xpRatio <= 0)
        {
            xpForeground.enabled = false;
        }
        else
        {
            xpForeground.enabled = true;
        }
    }

    private void SetHealthTexts()
    {
        currentHealthText.text = playerCurrentHealth.ToString();
        maxHealthText.text = playerMaxHealth.ToString();
    }

    private void SetManaTexts()
    {
        currentManaText.text = playerCurrentMana.ToString();
        maxManaText.text = playerMaxMana.ToString();
    }

    private void SetXPText()
    {
        levelText.text = "Lv. " + player.GetComponent<PlayerLeveling>().GetCurrentLevel().ToString();
    }
}
