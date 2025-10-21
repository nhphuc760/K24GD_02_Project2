using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;
public class ShopManagerScript : MonoBehaviour
{
    public int[,] shopItems = new int[5, 10];
    public float coin;
    public Text coinText;


    void Start()
    {
        if (coinText != null)
            coinText.text = "Coins: " + coin.ToString();
        else
            Debug.LogWarning("ShopManagerScript: coinText is not assigned in the inspector.");
        //ID 
        shopItems[1, 1] = 1;
        shopItems[1, 2] = 2;
        shopItems[1, 3] = 3;
        shopItems[1, 4] = 4;
        shopItems[1, 5] = 5;
        shopItems[1, 6] = 6;
        shopItems[1, 7] = 7;
        shopItems[1, 8] = 8;
        shopItems[1, 9] = 9;
        //Price
        shopItems[2, 1] = 10;
        shopItems[2, 2] = 25;
        shopItems[2, 3] = 25;
        shopItems[2, 4] = 5;
        shopItems[2, 5] = 15;
        shopItems[2, 6] = 30;
        shopItems[2, 7] = 50;
        shopItems[2, 8] = 100;
        shopItems[2, 9] = 100;
        //Quantity
        shopItems[3, 1] = 0;
        shopItems[3, 2] = 0;
        shopItems[3, 3] = 0;
        shopItems[3, 4] = 0;
        shopItems[3, 5] = 0;
        shopItems[3, 6] = 0;
        shopItems[3, 7] = 0;
        shopItems[3, 8] = 0;
        shopItems[3, 9] = 0;
        
    }

    public void Buy()
    {
        // Get the currently selected UI GameObject from the EventSystem
        if (EventSystem.current == null)
        {
            Debug.LogWarning("ShopManagerScript: No EventSystem.current found in scene.");
            return;
        }

        GameObject ButtonRef = EventSystem.current.currentSelectedGameObject;
        if (ButtonRef == null)
        {
            Debug.LogWarning("ShopManagerScript: No currently selected UI GameObject.");
            return;
        }

        var info = ButtonRef.GetComponent<ButtonInfo>();
        if (info == null)
        {
            Debug.LogWarning("ShopManagerScript: Selected GameObject does not have a ButtonInfo component.");
            return;
        }

        int itemID = info.itemID;

        // Validate itemID bounds before accessing the array
        if (itemID < 0 || itemID >= shopItems.GetLength(1))
        {
            Debug.LogWarning($"ShopManagerScript: itemID out of range: {itemID}");
            return;
        }

        int price = shopItems[2, itemID];
        if (coin >= price)
        {
            coin -= price;
            shopItems[3, itemID]++;
            if (coinText != null)
                coinText.text = "Coins: " + coin.ToString();
            info.quantityText.text = shopItems[3, itemID].ToString();
        }
        else
        {
            Debug.Log("ShopManagerScript: Not enough coins to buy item " + itemID);
        }
    }
}
