using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyText : MonoBehaviour
{
    GameObject player;

    [SerializeField] Text moneyText;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Update the money text so it always equals the player's money
        moneyText.text = FormatMoneyText(player.GetComponent<MoneyController>().GetPlayerMoney());
    }

    // Function that formats money text
    public string FormatMoneyText(int money)
    {
        string moneyString = money.ToString();

        // Add commas if the money is above $1,000
        if (money >= 100000)
        {
            moneyString = "$" + moneyString.Substring(0, 3) + "," + moneyString.Substring(3);
        }
        else if (money >= 10000)
        {
            moneyString = "$" + moneyString.Substring(0, 2) + "," + moneyString.Substring(2); 
        }
        else if (money >= 1000)
        {
            moneyString = "$" + moneyString.Substring(0, 1) + "," + moneyString.Substring(1);
        }
        else
        {
            moneyString = "$" + moneyString;
        }

        return moneyString;
    }
}
