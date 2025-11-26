using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Animal Database", menuName = "Game Data/Animal Database")]
public class AnimalDatabase : ScriptableObject
{
    public List<AnimalDataSO> allAnimalData;

    public AnimalDataSO GetAnimalDataByName(string name)
    {
        return allAnimalData.Find(x => x._itemName == name);
    }

    public AnimalDataSO GetAnimalDataByID(int id)
    {
        return allAnimalData.Find(x => x._id == id);
    }
    // Tùy chọn: Tự động tải từ thư mục Resources
    private void OnValidate()
    {
        allAnimalData = Resources.LoadAll<AnimalDataSO>("Items/Animal").ToList();
    }
}