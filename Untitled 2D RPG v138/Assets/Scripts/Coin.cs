using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    GameObject player;

    [SerializeField] GameObject moneyText;

    private int coinValue;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Function that sets the coin's value
    public void SetCoinValue(int value)
    {
        coinValue = value;
    }

    // Function that gets called when the player walks through the coin
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Add the amount to the player's money
            player.GetComponent<MoneyController>().AddToPlayerMoney(coinValue);

            // Instantiate money text
            ShowText("$" + coinValue.ToString(), moneyText);

            // Destroy this GameObject
            Destroy(gameObject);
        }
    }

    // Function for instantiating text (not going to use Health's because it doesn't make sense)
    public void ShowText(string theText, GameObject textItem)
    {
        GameObject instantiatedText = Instantiate(textItem, transform.position, Quaternion.identity);
        instantiatedText.GetComponentInChildren<TextMesh>().text = theText;
    }
}
