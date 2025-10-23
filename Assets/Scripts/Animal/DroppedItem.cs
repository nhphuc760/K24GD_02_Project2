using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public ItemDataSO itemData; // Item data để add vào inventory
    public int amount = 1; // Số lượng
    public float pickupDistance = 1.5f; // Khoảng cách nhặt

    private Transform player;

    void Start()
    {
        // Tìm player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance < pickupDistance)
            {
                // Nhặt item
                if (InventoryManager.Instance != null && itemData != null)
                {
                    InventoryManager.Instance.inventory.AddItem(itemData, amount);
                    InventoryManager.Instance.UpdateUI();
                }
                Destroy(gameObject);
            }
        }
    }
}