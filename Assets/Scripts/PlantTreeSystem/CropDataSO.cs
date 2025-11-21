using UnityEngine;

[CreateAssetMenu(fileName = "CropData", menuName = "Scriptable Objects/CropDataSO")]
public class CropDataSO : ItemDataSO
{
    public int sellPrice;
    public GameObject prefab;
    public override void OnValidate()
    {
        _description = $"Bán hoặc chế biến món ăn\nGiá bán: {sellPrice}$/1";
    }
}
