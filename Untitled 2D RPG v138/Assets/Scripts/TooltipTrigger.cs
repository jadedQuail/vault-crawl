using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float delayTime;

    public string header;

    [Multiline()]
    public string content;

    [Multiline()]
    public string attributes;

    // Tooltip is shown when the mouse hovers over the object this script is attached to
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(ShowTooltipAfterDelay(delayTime));
    }

    // Tooltip is hidden when the mouse moves away from the object this script is attached to
    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        TooltipSystem.Hide();
    }

    private IEnumerator ShowTooltipAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        TooltipSystem.Show(content, header, attributes);
    }

    // Function for setting the header
    public void SetHeader(string theText)
    {
        header = theText;
    }

    // Function for setting the content
    public void SetContent(string theText)
    {
        content = theText;
    }

    // Function for setting the attributes
    public void SetAttributes(string theText)
    {
        attributes = theText;
    }
}
