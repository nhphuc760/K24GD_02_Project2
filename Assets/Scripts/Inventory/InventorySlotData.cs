using UnityEngine;
using Newtonsoft.Json;
public class InventorySlotData 
{
    public int slotIndex;
    public int _idDataSO;
    public int quantity;
    public string dataRuntime;

    public InventorySlotData()
    {

    }

    public InventorySlotData(int slotIndex, int idDataSO, int quantity, string dataRuntime) 
    {
        this.slotIndex = slotIndex;
        this._idDataSO = idDataSO;
        this.quantity = quantity;
        this.dataRuntime = dataRuntime;
    }

    public string SerializeToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}
