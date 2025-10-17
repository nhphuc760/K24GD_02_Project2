using UnityEngine;
using Newtonsoft.Json;
public class InventorySlotData 
{
    public int slotIndex;
    public int _idDataSO;
    public int quantity;

    public InventorySlotData()
    {

    }

    public InventorySlotData(int slotIndex, int idDataSO, int quantity)
    {
        this.slotIndex = slotIndex;
        this._idDataSO = idDataSO;
        this.quantity = quantity;
    }

    public string SerializeToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}
