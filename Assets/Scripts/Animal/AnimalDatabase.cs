using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Animal Database", menuName = "Game Data/Animal Database")]
public class AnimalDatabase : ScriptableObject
{
    public List<AnimalDataSO> allAnimalData;

    public AnimalDataSO GetAnimalDataByName(string name)
    {
        if (allAnimalData == null) return null;
        foreach (AnimalDataSO data in allAnimalData)
        {
            if (data != null && data._itemName == name)
            {
                return data;
            }
        }
        Debug.LogWarning("AnimalDatabase: AnimalData with ID " + name + " not found.");
        return null;
    }

    // Tùy chọn: Tự động tải từ thư mục Resources
    // private void OnValidate()
    // {
    //     allAnimalData = Resources.LoadAll<AnimalDataSO>("Animals").ToList();
    // }
}