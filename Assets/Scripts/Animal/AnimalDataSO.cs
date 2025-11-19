using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AnimalData", menuName = "Scriptable Objects/AnimalDataSO")]
public class AnimalDataSO : ScriptableObject 
{
    [Header("Info")]
    public string _id; // ID duy nhất, ví dụ: "chicken"
    public string animalName;
    public GameObject animalPrefab; // Prefab con vật (chứa script FarmAnimal.cs)

    [Header("Shop")]
    public int purchasePrice; // Giá mua ở shop

    [Header("Production")]
    public float secondsToProduce; // Số giây để ra sản phẩm
    public ProductDataSO productData; // "Sản phẩm" nó tạo ra 

    [Header("Visuals")]
    public Sprite harvestIndicator; // Icon báo sẵn sàng thu hoạch
}