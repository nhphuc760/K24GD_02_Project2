using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;

public class Inventory 
{
    public List<InventorySlot> itemSlots = new List<InventorySlot>();
    InventoryDataBaseSO itemDataBase;
    public event Action OnExpandSlot;
    public Inventory(int size, InventoryDataBaseSO itemDataBasae)
    {
        for (int i = 0; i < size; i++)
        {
            itemSlots.Add(new InventorySlot());
        }
        this.itemDataBase = itemDataBasae;
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
            end.Assign(start.ItemData, start.quantity, start.dataRuntime);
            start.Clear();
            return;
        }
        if (start.ItemData != end.ItemData)
        {
            var tmpItem = end.ItemData;
            var tmpQuantity = end.quantity;
            var tmpDataRuntime = end.dataRuntime;
            end.Assign(start.ItemData, start.quantity, start.dataRuntime);
            start.Assign(tmpItem, tmpQuantity, tmpDataRuntime);

        }
    }

    public bool TryDropItem(InventorySlot start, InventorySlot end)
    {
       try{ 
        if (start.ItemData == end.ItemData && start.ItemData.isStackable)
        {
            int totalQuantity = start.quantity + end.quantity;
            if (totalQuantity <= start.ItemData._maxStack)
            {
                return true;
            }
        }
        if (end.IsEmpty)
        {
            return true;
        }
            if (start.ItemData != end.ItemData)
            {
                return false;
            } 
        }catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        return false;
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
    public bool AddItem(ItemDataSO item, int quantity = 1, DataRunTimeItem dataRunTimeItem = null)
    {
        // Try stacking first
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
                DataRunTimeItem data = dataRunTimeItem;
                    if (item.runTimeItemType != RunTimeItemType.None && data == null)
                    {
                        Type dataType = RuntimeDataRegistry.GetType(item.runTimeItemType.ToString());
                        if (dataType != null)
                        {
                            try
                            {
                                DataRunTimeItem instance = (DataRunTimeItem)Activator.CreateInstance(dataType);
                                instance.Init(item);
                                data = instance; // <-- assign instance to data so it is stored to slot
                            }
                            catch (Exception e)
                            {
                                Debug.LogWarning($"Inventory.AddItem: failed to create runtime data for item {item._itemName}: {e}");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Inventory.AddItem: runtime type for '{item.runTimeItemType}' not registered.");
                        }
                    }
                
                    slot.Assign(item, quantity, data);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Xóa Item theo index slot
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <param name="quantity"></param>
    public void RemoveItem(int slotIndex, int quantity)
    {
        itemSlots[slotIndex].Remove(quantity);
        GameEventManager.Ins.inventoryEvent.RemoveItemCompleted(itemSlots[slotIndex].IsEmpty);
    }


    /// <summary>
    /// Xóa item theo itemData
    /// </summary>
    /// <param name="itemDataSO"></param>
    /// <param name="quantity"></param>
    public void RemoveItem(ItemDataSO itemDataSO, int quantity)
    {
        if (itemDataSO == null) return;
        if (quantity <= 0) return;

        // Nếu không đủ item thì không xóa
        if (!HasItem(itemDataSO, quantity))
        {
            Debug.LogWarning($"Inventory.RemoveItem: not enough '{itemDataSO.name}' to remove ({quantity} requested).");
            return;
        }

        int remaining = quantity;

        // Đối với items có thể stack: giảm dần số lượng trong các slot chứa item đó.
        // Đối với non-stackable: mỗi slot chứa 1 item -> clear slot.
        for (int i = 0; i < itemSlots.Count && remaining > 0; i++)
        {
            var slot = itemSlots[i];
            if (slot == null || slot.ItemData != itemDataSO) continue;

            if (itemDataSO.isStackable)
            {
                if (slot.quantity > remaining)
                {
                    slot.Remove(remaining); // giảm một phần
                    remaining = 0;
                }
                else
                {
                    // remove toàn bộ slot.quantity
                    int toRemove = slot.quantity;
                    slot.Remove(toRemove); // hoặc slot.Clear();
                    remaining -= toRemove;
                }
            }
            else
            {
                // non-stackable: mỗi slot chứa 1 unit (quantity có thể =1)
                slot.Clear();
                remaining -= 1;
            }
        }

        if (remaining > 0)
        {
            // Không nên xảy ra vì đã kiểm HasItem phía trên, nhưng log để dễ debug
            Debug.LogWarning($"Inventory.RemoveItem: leftover to remove = {remaining} for '{itemDataSO.name}'.");
        }
    }

    /// <summary>
    /// Kiểm tra inventory trống hay không
    /// </summary>
    /// <returns></returns>

    public bool CheckEmpty()
    {
        foreach (var item in itemSlots)
        {
            if (item.ItemData != null) return false;
        }
        return true;
    }

    // Kiểm tra xem inventory có ít nhất `quantity` item tương ứng không.
    // Trả về false nếu item == null hoặc quantity <= 0.
    /// <summary>
    /// Kiểm tra trong inventory có ít nhất quantity item tương ứng không
    /// </summary>
    /// <param name="item"></param>
    /// <param name="quantity"></param>
    /// <returns></returns>
    public bool HasItem(ItemDataSO item, int quantity)
    {
        if (item == null) return false;
        if (quantity <= 0) return false;

        // Tổng số lượng hiện có của item trong inventory (dùng field quantity của slot)
        int total = 0;
        foreach (var slot in itemSlots)
        {
            if (slot == null) continue;
            if (slot.ItemData == item)
            {
                total += slot.quantity;
            }
        }

        return total >= quantity;
    }

    /// <summary>
    /// </summary>
    /// param name="idInventory">ID của kho</param>
    public async Task SaveData(string keyInventory)
    {
        try
        {
            List<InventorySlotData> itemDataSlots = new List<InventorySlotData>();
            foreach (var child in itemSlots)
            {
                if (!child.IsEmpty)
                {
                    InventorySlotData data = new InventorySlotData();
                    data._idDataSO = child.ItemData._id;
                    data.quantity = child.quantity;
                    data.slotIndex = itemSlots.IndexOf(child);
                    data.dataRuntime = (child.dataRuntime == null || child.ItemData.runTimeItemType.Equals(RunTimeItemType.None)) ? null : child.dataRuntime.SerializeData();
                    itemDataSlots.Add(data);
                }
            }
            await Save_Load_Firebase.SaveData($"Inventory/{keyInventory}/SlotAmount", itemSlots.Count);
            await Save_Load_Firebase.SaveData($"Inventory/{keyInventory}/Database", JsonConvert.SerializeObject(itemDataSlots));
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Inventory.SaveData error: {e}");
        }
    }

    public async Task LoadData(string keyInventory)
    {
        if (itemSlots.Count == 0) return;

        DataSnapshot task;
        try
        {
            task = await Save_Load_Firebase.LoadData($"Inventory/{keyInventory}/Database");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Inventory.LoadData: firebase call failed: {e}");
            return;
        }

        if (task == null)
        {
            Debug.LogWarning("Inventory.LoadData: returned snapshot is null");
            return;
        }

        if (!task.Exists) return;

        string json = task.Value?.ToString();
        if (string.IsNullOrEmpty(json)) return;

        List<InventorySlotData> remoteData;
        try
        {
            remoteData = JsonConvert.DeserializeObject<List<InventorySlotData>>(json);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Inventory.LoadData: failed to parse JSON: {e}");
            return;
        }
        if (remoteData == null) return;

        for (int i = 0; i < remoteData.Count; i++)
        {
            var child = remoteData[i];
            // validate slotIndex
            if (child == null)
            {
                Debug.LogWarning($"Inventory.LoadData: remoteData[{i}] is null, skipping.");
                continue;
            }
            if (child.slotIndex < 0 || child.slotIndex >= itemSlots.Count)
            {
                Debug.LogWarning($"Inventory.LoadData: slotIndex out of range ({child.slotIndex}), skipping.");
                continue;
            }

            // safe-get ItemDataSO (itemDataBase may be null)
            ItemDataSO dataSo = null;
            if (itemDataBase != null)
                dataSo = itemDataBase.GetDataByID(child._idDataSO);

            if (dataSo == null)
            {
                Debug.LogWarning($"Inventory.LoadData: ItemDataSO with id {child._idDataSO} not found in database. Skipping slot {child.slotIndex}.");
                continue;
            }

            DataRunTimeItem runtimeInstance = null;
            // Only try to deserialize runtime string when present and when the item declares a runtime type
            if (!string.IsNullOrEmpty(child.dataRuntime) && dataSo.runTimeItemType != RunTimeItemType.None)
            {
                Type runtimeType = RuntimeDataRegistry.GetType(dataSo.runTimeItemType.ToString());
                if (runtimeType != null)
                {
                    try
                    {
                        runtimeInstance = (DataRunTimeItem)JsonConvert.DeserializeObject(child.dataRuntime, runtimeType);
                        // If deserialization returned null, attempt to new-up and Init as fallback
                        if (runtimeInstance == null)
                        {
                            runtimeInstance = (DataRunTimeItem)Activator.CreateInstance(runtimeType);
                            runtimeInstance.Init(dataSo);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"Inventory.LoadData: failed to deserialize runtime data for slot {child.slotIndex}, id {child._idDataSO}: {e}");
                        runtimeInstance = null;
                    }
                }
                else
                {
                    Debug.LogWarning($"Inventory.LoadData: runtime type '{dataSo.runTimeItemType}' is not registered. Skipping runtime data for slot {child.slotIndex}.");
                }
            }

            // Assign slot data (use Assign to keep consistent behavior)
            try
            {
                itemSlots[child.slotIndex].Assign(dataSo, child.quantity, runtimeInstance);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Inventory.LoadData: failed to assign slot {child.slotIndex}: {e}");
            }
        }
    }

    public int GetTotal(ItemDataSO item)
    {
        if (item == null) return 0;
        int total = 0;
        foreach (var slot in itemSlots)
        {
            if (slot == null) continue;
            if (slot.ItemData == item)
                total += slot.quantity;
        }
        return total;
    }
}
