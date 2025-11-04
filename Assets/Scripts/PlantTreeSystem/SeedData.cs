using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SeedData", menuName = "Scriptable Objects/SeedData")]
public class SeedData : ItemDataSO 
{
    [Header("Crop Info")]
    public int DaysToGrow; // Number of days to fully grow
    public CropDataSO cropData; //Dữ liệu cây thu hoạch
    [Header("Crop Stages Sprites adn prefab")]
    public List<Sprite> growhtSprites; // Sprite for the mature stage
    [Header("Crop Economic Value")]
    public int purchasePrice; // Price to buy the seed
    public int sellPrice; // Price to sell the mature crop
    public int yield;// sản lượng thu được mỗi khi thu hoạch
}
