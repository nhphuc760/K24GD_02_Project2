using System;
using Newtonsoft.Json;
using UnityEngine;

public class KitchenData
{
    public DateTime coolDownStart;
    public DateTime coolDownEnd;
    public bool isCooking;
    public int _idItemKitchen;
    public KitchenData()
    {

    }
    public KitchenData(DateTime coolDownStart, DateTime coolDownEnd, bool isCooking, int idItemKitchen)
    {
        this.coolDownStart = coolDownStart;
        this.coolDownEnd = coolDownEnd;
        this.isCooking = isCooking;
        _idItemKitchen = idItemKitchen;
    }
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
