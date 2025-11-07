using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;


[CreateAssetMenu(fileName = "New Crop Database", menuName = "Game Data/Crop Database")]
public class SeedDatabase: ScriptableObject
{
    //Danh sach các loại cây trồng trong game
    public List<SeedDataSO> allSeedData;

    //tìm cropData theo id
    public SeedDataSO GetSeedDataByID(int id)
    {
        if (allSeedData == null)
            return null;
        foreach(SeedDataSO data in allSeedData)
        {
            if (data != null && data._id == id)
            {
                return data;
            }
        }
        Debug.LogWarning("CropDatabase: CropData with ID " + id + " not found.");
        return null;
    }

    private void OnValidate()
    {
        allSeedData = Resources.LoadAll<SeedDataSO>("Items").ToList();
    }

}
