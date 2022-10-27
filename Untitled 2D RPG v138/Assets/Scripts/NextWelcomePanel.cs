using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextWelcomePanel : MonoBehaviour
{
    [Header("Panel To Open")]
    [SerializeField] GameObject nextPanel;

    public void OpenNextPanel()
    {
        // Open the next panel
        nextPanel.SetActive(true);

        // Close this panel
        gameObject.SetActive(false);
    }
}
