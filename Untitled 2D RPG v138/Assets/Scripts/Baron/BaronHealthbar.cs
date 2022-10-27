using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaronHealthbar : MonoBehaviour
{
    [SerializeField] GameObject healthBar;
    private Transform bar;

    // Start is called before the first frame update
    private void Start()
    {
        bar = transform.Find("Bar");
    }

    public void SetSize(float health, float maxHealth)
    {
        float sizeNormalized = health / maxHealth;

        // Deactivate the bar if it's full
        if (sizeNormalized >= 1f)
        {
            healthBar.SetActive(false);
        }
        else
        {
            healthBar.SetActive(true);
        }

        // Never go below 0
        if (sizeNormalized <= 0f) { sizeNormalized = 0f; }

        // Set the size of the bar
        if (bar != null)
        {
            bar.localScale = new Vector3(sizeNormalized, 1f);
        }
    }

}
