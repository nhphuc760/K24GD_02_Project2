using UnityEngine;

[CreateAssetMenu(fileName = "ItemShopDataSO", menuName = "Scriptable Objects/ShopItemSO")]
public class ItemShopDataSO : ItemDataSO
{
    [Tooltip("Giá bán trong shop")]
    public int purchase_Price;
}
