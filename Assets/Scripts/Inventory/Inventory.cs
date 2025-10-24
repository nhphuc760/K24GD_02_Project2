using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using UnityEngine;
using Newtonsoft.Json;
using Firebase.Extensions;
using System.Threading.Tasks;
using Firebase.Auth;
using System;
public class Inventory 
{
    public List<InventorySlot> itemSlots = new List<InventorySlot>();
    InventoryDataBaseSO itemDataBase;
    DatabaseReference reference;
    public event Action OnExpandSlot;
    public Inventory(int size, InventoryDataBaseSO itemDataBasae)
    {
        for (int i = 0; i < size; i++)
        {
            itemSlots.Add(new InventorySlot());
        }
        this.itemDataBase = itemDataBasae;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    /// <summary>
    /// Gộp, Hoán đổi, Đổi chỗ vật phẩm
    /// </summary>
    /// <param name="start">Slot bắt đầu kéo thả</param>
    /// <param name="end">Slot mà ta thả vào</param>
    public void MergeItem(InventorySlot start, InventorySlot end)
    {
        if (start.IsEmpty) return;
        if (start.ItemData == end.ItemData && start.ItemData.isStackable)
        {
            int totalQuantity = start.quantity + end.quantity;
            if (totalQuantity <= start.ItemData._maxStack)
            {
                end.quantity = totalQuantity;
                start.Clear();
            }
            else
            {
                end.quantity = end.ItemData._maxStack;
                start.quantity = totalQuantity - start.ItemData._maxStack;
            }
            return;
        }
        if (end.IsEmpty)
        {
            end.Assign(start.ItemData, start.quantity);
            start.Clear();
            return;
        }
        if (start.ItemData != end.ItemData)
        {
            var tmpItem = end.ItemData;
            var tmpQuantity = end.quantity;
            end.Assign(start.ItemData, start.quantity);
            start.Assign(tmpItem, tmpQuantity);
        }
    }

    public void AddSlot()
    {
        itemSlots.Add(new InventorySlot());
        OnExpandSlot?.Invoke();
    }

    /// <summary>
    /// Thêm vật phẩm vào kho
    /// </summary>
    /// param name="item">ScriptableObject của Item</param>
    public bool AddItem(ItemDataSO item, int quantity = 1)
    {
        foreach (var slot in itemSlots)
        {
            if (slot.ItemData == item && slot.ItemData.isStackable)
            {
                if (slot.quantity >= item._maxStack) continue;
                slot.quantity = Mathf.Min(slot.quantity + quantity, item._maxStack);
                return true;
            }
        }

        foreach (var slot in itemSlots)
        {
            if (slot.IsEmpty)
            {
                slot.Assign(item, quantity);
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// </summary>
    /// param name="idInventory">ID của kho</param>
    public async Task SaveData(string keyInventory)
    {
        List<InventorySlotData> itemDataSlots = new List<InventorySlotData>();
        foreach (var child in itemSlots)
        {
            if (!child.IsEmpty)
            {
                InventorySlotData data = new InventorySlotData
                {
                    _idDataSO = child.ItemData._id,
                    quantity = child.quantity,
                    slotIndex = itemSlots.IndexOf(child)
                };
                itemDataSlots.Add(data);
            }
        }
        await reference.Child(Save_Load_Firebase.GetUserID()).Child($"Inventory/{keyInventory}/SlotAmount").SetValueAsync(itemSlots.Count);
        await reference.Child(Save_Load_Firebase.GetUserID()).Child($"Inventory/{keyInventory}/Database").SetValueAsync(JsonConvert.SerializeObject(itemDataSlots));
    }
    public void LoadData(string keyInventory)
    {
        if (itemSlots.Count == 0) return;
        reference.Child(Save_Load_Firebase.GetUserID()).Child($"Inventory/{keyInventory}/Database").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (!snapshot.Exists) return;
                string json = snapshot.Value.ToString();
                var remoteData = JsonConvert.DeserializeObject<List<InventorySlotData>>(json);
                if (remoteData == null) return;
                foreach (var child in remoteData)
                {
                    itemSlots[child.slotIndex].ItemData = itemDataBase.GetDataByID(child._idDataSO);
                    itemSlots[child.slotIndex].quantity = child.quantity;
                }
                InventoryManager.Instance.UpdateUI();
            }

        });
    }
}
