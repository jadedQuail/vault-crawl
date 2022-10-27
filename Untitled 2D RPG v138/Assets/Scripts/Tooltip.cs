using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public TextMeshProUGUI attributesField;

    public LayoutElement layoutElement;

    public int characterWrapLimit;

    public RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Function that sets the text of the tooltip
    public void SetText(string content, string header = "", string attributes = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        if (string.IsNullOrEmpty(attributes))
        {
            attributesField.gameObject.SetActive(false);
        }
        else
        {
            attributesField.gameObject.SetActive(true);
            attributesField.text = attributes;
        }

        contentField.text = content;

        int headerLength = headerField.text.Length;
        int contentLength = contentField.text.Length;
        int attributesLength = attributesField.text.Length;

        // Ternary operator
        layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit || attributesLength > characterWrapLimit) ? true : false;
    }

    private void Update()
    {
        // Set the position of the tooltip to be where the mouse is
        Vector2 position = Input.mousePosition;
        transform.position = position;
    }
}
