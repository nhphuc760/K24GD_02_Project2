using UnityEngine;

[CreateAssetMenu(fileName = "ProductData", menuName = "Scriptable Objects/ProductDataSO")]
public class ProductDataSO : ItemDataSO // Kế thừa từ ItemDataSO
{
    [Header("Product Info")]
    public GameObject prefab; // Prefab để thả ra đất (nếu cần)
    public string Description;
    public override void OnValidate()
    {
        _description = Description + $"\nGiá bán: {sell}$/1";
    }
}