using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryDataBase", menuName = "Scriptable Objects/Inventory/InventoryDataBase")]
public class InventoryDataBaseSO : ScriptableObject
{
    public List<ItemDataSO> itemDataSOs = new List<ItemDataSO>();
    public ItemDataSO GetDataByID(int id)
    {
        return itemDataSOs.Find(x => x._id == id);
    }
    private void OnValidate()
    {
        itemDataSOs = Resources.LoadAll<ItemDataSO>("Items").ToList();
    }
}
