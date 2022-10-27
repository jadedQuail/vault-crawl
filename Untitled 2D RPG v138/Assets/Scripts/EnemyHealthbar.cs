using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar : MonoBehaviour
{
    [SerializeField] Image healthbarBackground;
    [SerializeField] Image healthbarForeground;
    [SerializeField] Vector3 offset;
    [SerializeField] Transform parentTransform;

    Camera mainCamera;

    GameObject player;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        mainCamera = FindMainCamera();
    }

    public void SetEnemyHealthbar(float health, float maxHealth)
    {
        // Only make the healthbar active if not at full health (and deactivate it if the player dies)
        healthbarBackground.gameObject.SetActive(health < maxHealth);
        healthbarForeground.gameObject.SetActive(health < maxHealth);

        float healthRatio = health / maxHealth;

        healthbarForeground.fillAmount = healthRatio;

        if (healthRatio <= 0)
        {
            healthbarForeground.enabled = false;
        }
        else
        {
            healthbarForeground.enabled = true;
        }
    }

    // Function that deactivates the enemy's healthbar
    public void DeactivateEnemyHealthbar()
    {
        healthbarBackground.gameObject.SetActive(false);
        healthbarForeground.gameObject.SetActive(false);
    }

    // Function that finds the main camera
    public Camera FindMainCamera()
    {
        return GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move the healthbar to be above the enemy
        healthbarBackground.transform.position = mainCamera.WorldToScreenPoint(parentTransform.position + offset);
        healthbarForeground.transform.position = mainCamera.WorldToScreenPoint(parentTransform.position + offset);
    }
}
