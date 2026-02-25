using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemShopDataSO", menuName = "Scriptable Objects/ShopItemSO")]
public class ItemShopDataSO : ItemDataSO
{
    [Tooltip("Giá bán trong shop")]
    public int purchase_Price;
    [Tooltip("Hành vi của dữ liệu khi mua")]
    public TypeItemShop typeItemShop;
    public enum TypeItemShop
    {
        [Tooltip("Đối tượng sẽ thêm vào Inventory khi mua")]
        AddInven,
        [Tooltip("Đối tượng sẽ thực hiện chức năng riêng khi mua")]
        Separate
    }
}
