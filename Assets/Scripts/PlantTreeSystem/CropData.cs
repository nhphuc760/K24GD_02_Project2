using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CropData", menuName = "Scriptable Objects/CropData")]
public class CropData : ScriptableObject
{
    [Header("Crop Info")]
    public string cropName;
    public Sprite icon; // Icon for UI representation
    public string cropDescription;
    public int DaysToGrow; // Number of days to fully grow

    [Header("Crop Stages Sprites adn prefab")]
    public Sprite seedSprite; // Sprite for the seed stage
    public List<Sprite> growhtSprites; // Sprite for the mature stage
    public GameObject cropPrefab; // Prefab for the crop to instantiate in the game world

    [Header("Crop Economic Value")]
    public int purchasePrice; // Price to buy the seed
    public int sellPrice; // Price to sell the mature crop
}
