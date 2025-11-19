using Newtonsoft.Json;
using UnityEngine;

public class ToolRunTimeData : DataRunTimeItem
{
    public int currentDurability;

    public override void Init(ItemDataSO itemDataSO)
    {
      currentDurability = ((ToolDataSO)itemDataSO).maxDurability;
    }

    public override string SerializeData()
    {
       return JsonConvert.SerializeObject(this);
    }

    public override string ToString()
    {
       return $"Độ bền: {currentDurability}";
    }
    
}
