using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem current;

    public Tooltip tooltip;

    void Start()
    {
        // Keep this object in all scenes
        if (current == null)
        {
            current = this;
        }
        else
        {
            if (current != this)
            {
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);
        Hide();
    }

    private void Update()
    {
        // If no menus are open, there's no reason for a tooltip to be activated
        if (!GameObject.FindWithTag("HUD").GetComponent<HUD>().AnyMenuOpen())
        {
            Hide();
        }
    }

    // Function that shows the tooltip
    public static void Show(string content, string header = "", string attributes = "")
    {
        current.tooltip.SetText(content, header, attributes);
        current.tooltip.gameObject.SetActive(true);
    }

    // Function that hides the tooltip
    public static void Hide()
    {
        current.tooltip.gameObject.SetActive(false);
    }
}
