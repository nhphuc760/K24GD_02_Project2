using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopDatabaseSO", menuName = "Scriptable Objects/ShopDatabase")]
public class ShopDataBaseSO : ScriptableObject
{
    public List<ItemShopDataSO> shopDatabase;
    public ItemShopDataSO GetItemShopByID(int id)
    {
        return shopDatabase.Find(x => x._id.Equals(id));
    }

    private void OnValidate()
    {
        shopDatabase = Resources.LoadAll<ItemShopDataSO>("Items").ToList();
    }
}
