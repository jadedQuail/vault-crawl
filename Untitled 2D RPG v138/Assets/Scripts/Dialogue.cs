using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    GameObject gameController;

    [SerializeField] Text titleText;
    [SerializeField] Text dialogueText;
    [SerializeField] Image dialogueFace;

    private List<string> dialogueLines;
    private List<int> questionIndicationList;
    private List<int> endCaseList;

    private string currentLine;
    private int dialogueIndex;

    private string dialogueStatus;

    // Conditions for "Yes/No" branches
    private bool buttonsOpen = false;
    private string resetPath = "";

    private bool dialogueEnding = false;

    private void Start()
    {
        gameController = GameObject.FindWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        SetDialogueStatus();
        PopulateDialogueText();
    }

    // PRIVATE FUNCTIONS

    // Function for populating the dialogue text
    private void PopulateDialogueText()
    {
        dialogueText.text = currentLine;
    }

    // A function for setting the status of the dialogue
    private void SetDialogueStatus()
    {
        if (dialogueLines != null)
        {
            // Dialogue is only 1 line
            if (dialogueLines.Count == 1)
            {
                dialogueStatus = "end";
            }
            // First line
            else if (dialogueIndex == 0)
            {
                dialogueStatus = "start";
            }
            // Last line
            else if (dialogueIndex >= dialogueLines.Count - 1)
            {
                dialogueStatus = "end";
            }
            // Override in which dialogue is marked as something that will end manually
            else if (dialogueEnding == true)
            {
                dialogueStatus = "end";
            }
            // Middle lines
            else
            {
                dialogueStatus = "middle";
            }
        }
    }

    // Function for checking if we've reached an end case
    private void CheckForEndCase(int i)
    {
        if (endCaseList[i] == 1)
        {
            dialogueEnding = true;
        }
    }

    // PUBLIC FUNCTIONS

    // Function for grabbing dialogue from NPCs
    public void SetDialogue(List<string> dialogue, List<int> questionIndicators, List<int> endCases)
    {
        dialogueLines = dialogue;
        questionIndicationList = questionIndicators;
        endCaseList = endCases;

        // Initialize the index
        dialogueIndex = 0;

        // For the case where the first line is a question
        if (questionIndicationList[dialogueIndex] == 1)
        {
            GetComponent<HUD>().EnableActionButtonsCluster(true);
            buttonsOpen = true;
        }

        // Check if the first line is an end case
        CheckForEndCase(dialogueIndex);

        // Load in the line of dialogue
        currentLine = dialogueLines[dialogueIndex];
    }

    // Function for grabbing the face sprite from NPCs
    public void SetDialogueFace(Sprite face)
    {
        dialogueFace.sprite = face;
    }

    // Function for setting the NPC's name
    public void SetNPCName(string name)
    {
        titleText.text = name;
    }

    // Function that advances the dialogue text
    public void AdvanceDialogueText()
    {
        dialogueIndex += 1;

        // Check if the advanced case is an end case
        CheckForEndCase(dialogueIndex);

        // Indicates that we have a question
        if (questionIndicationList[dialogueIndex] == 1)
        {
            GetComponent<HUD>().EnableActionButtonsCluster(true);
            buttonsOpen = true;
        }

        // Indicates that we have a closed path that needs to be skipped
        if (questionIndicationList[dialogueIndex] == 999)
        {
            while (questionIndicationList[dialogueIndex] == 999)
            {
                dialogueIndex += 1;
            }
        }

        // Load in the line of dialogue

        if (dialogueIndex < dialogueLines.Count)  // Only load up dialogue if the index is still in range
        {
            currentLine = dialogueLines[dialogueIndex];
        }
    }

    // Function that gives the dialogue status
    public string GetDialogueStatus()
    {
        return dialogueStatus;
    }

    // Function that indicates whether or not the dialogue has entered an option-based path
    public bool GetButtonsOpen()
    {
        return buttonsOpen;
    }

    // Function for selecting a "Yes" path on dialogue
    public void SetYesPath()
    {
        if (gameController.GetComponent<GameController>().GetCanOpenMenus())
        {
            // Block the "No" path with code 999
            for (int i = 0; i < dialogueLines.Count; i++)
            {
                if (questionIndicationList[i] == 3)
                {
                    questionIndicationList[i] = 999;
                }
            }

            AdvanceDialogueText();
            buttonsOpen = false;
            resetPath = "no";
            GetComponent<HUD>().EnableActionButtonsCluster(false);
        }
    }

    // Function for selecting a "No" path on dialogue
    public void SetNoPath()
    {
        if (gameController.GetComponent<GameController>().GetCanOpenMenus())
        {
            // Block the "Yes" path with code 999
            for (int i = 0; i < dialogueLines.Count; i++)
            {
                if (questionIndicationList[i] == 2)
                {
                    questionIndicationList[i] = 999;
                }
            }

            AdvanceDialogueText();
            buttonsOpen = false;
            resetPath = "yes";
            GetComponent<HUD>().EnableActionButtonsCluster(false);
        }
    }

    // Function for reconstructing the dialogue lists so they can be used again
    public void ResetDialogue()
    {
        // Reset dialogue ending flag
        dialogueEnding = false;

        // Reset "yes" and "no" paths
        if (resetPath == "yes")
        {
            for (int i = 0; i < dialogueLines.Count; i++)
            {
                if (questionIndicationList[i] == 999)
                {
                    questionIndicationList[i] = 2;
                }
            }
        }
        else if (resetPath == "no")
        {
            for (int i = 0; i < dialogueLines.Count; i++)
            {
                if (questionIndicationList[i] == 999)
                {
                    questionIndicationList[i] = 3;
                }
            }
        }
    }
}
