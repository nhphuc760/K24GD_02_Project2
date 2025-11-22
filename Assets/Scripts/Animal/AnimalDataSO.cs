using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AnimalData", menuName = "Scriptable Objects/AnimalDataSO")]
public class AnimalDataSO : ItemShopDataSO // có thể được mua trong shop.
{
    [Header("Info")]
    public GameObject animalPrefab; // Prefab con vật (chứa script FarmAnimal.cs)
    [Header("Production")]
    [Tooltip("Thời gian tạo ra sản phẩm, tính bằng giây")]
    public float secondsToProduce; 
    [Tooltip("Sản phẩm tạo ra")]
    public ProductDataSO productData; 

    [Header("Visuals")]
    [Tooltip("Icon báo sẵn sàng lấy sản phẩm")]
    public Sprite harvestIndicator;
    public override bool CheckConditionBuy(int quantity)
    {
        return base.CheckConditionBuy(quantity);
    }
    public override void SeparateBuy(int quantity)
    {
        base.SeparateBuy(quantity);
    }
}