using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;


[CreateAssetMenu(fileName = "New Crop Database", menuName = "Game Data/Crop Database")]
public class CropDatabase: ScriptableObject
{
    //Danh sach các loại cây trồng trong game
    public List<CropData> allCropData;

    //tìm cropData theo id
    public CropData GetCropDataByID(string id)
    {
        if (allCropData == null)
            return null;
        foreach(CropData data in allCropData)
        {
            if (data != null && data.name == id)
            {
                return data;
            }
        }
        Debug.LogWarning("CropDatabase: CropData with ID " + id + " not found.");
        return null;
    }
    
}
