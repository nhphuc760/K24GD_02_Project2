using NUnit.Framework;
using UnityEngine;

public class ToolKit : MonoBehaviour
{


    ItemDataSO curChoosing;
    public Inventory inventory;
    private void Awake()
    {
        inventory = new Inventory(9, null);
        GameEventManager.Ins.inventoryEvent.onInitSuccess += InitInventorySuccess;
    }

    private void InitInventorySuccess(Inventory inventory)
    {

        // Mỗi lần khởi tạo sẽ lấy 9 phần tử đầu tiên trong inventory.
        for (int i = 0; i < 9; i++)
        {
            this.inventory.itemSlots[i] = inventory.itemSlots[i];
        }

    }
}
