using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonInfo : MonoBehaviour
{
    public int itemID;
    public Text priceText;
    public Text quantityText;
    public GameObject coinIcon; // Icon đồng xu hiển thị bên cạnh price
    public float rotationSpeed = 100f; // Tốc độ quay của icon đồng xu
    public GameObject ShopManager;
    private ShopManagerScript shopManagerScript;

    void Awake()
    {
        if (ShopManager != null)
        {
            shopManagerScript = ShopManager.GetComponent<ShopManagerScript>();
            if (shopManagerScript == null)
                Debug.LogWarning("ButtonInfo: ShopManager GameObject does not have a ShopManagerScript component.");
        }
        else
        {
            // Try to find one in the scene as a fallback
            shopManagerScript = UnityEngine.Object.FindFirstObjectByType<ShopManagerScript>();
            if (shopManagerScript == null)
                Debug.LogWarning("ButtonInfo: No ShopManager assigned and none found in scene.");
            else
                Debug.Log("ButtonInfo: Auto-assigned ShopManagerScript from scene.");
        }

        if (priceText == null)
            Debug.LogWarning($"ButtonInfo (itemID={itemID}): priceText is not assigned.");
        if (quantityText == null)
            Debug.LogWarning($"ButtonInfo (itemID={itemID}): quantityText is not assigned.");
    }

    void Update()
    {
        if (shopManagerScript == null) return;

        int maxCols = shopManagerScript.shopItems.GetLength(1);
        if (itemID < 0 || itemID >= maxCols)
        {
            Debug.LogWarning($"ButtonInfo: itemID {itemID} is out of range (0..{maxCols - 1}).");
            return;
        }

        if (priceText != null)
            priceText.text = shopManagerScript.shopItems[2, itemID].ToString();
        if (quantityText != null)
            quantityText.text = "Quantity: " + shopManagerScript.shopItems[3, itemID].ToString();

        // Quay vòng icon đồng xu nếu có
        if (coinIcon != null)
        {
            Image img = coinIcon.GetComponent<Image>();
            if (img != null)
            {
                img.rectTransform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
            }
        }
    }
}
