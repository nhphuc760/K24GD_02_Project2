using UnityEngine;

[CreateAssetMenu(fileName = "ProductData", menuName = "Scriptable Objects/ProductDataSO")]
public class ProductDataSO : ItemDataSO // Kế thừa từ ItemDataSO
{
    [Header("Product Info")]
    public int sellPrice;
    public GameObject prefab; // Prefab để thả ra đất (nếu cần)
}