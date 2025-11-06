using System.Collections.Generic;
using UnityEngine;


//lớp chính chứa toàn bộ dữ liệu gane
[System.Serializable]
public class GameData
{
    public string selectedCharacterName;          // Để lưu tên người chơi đã nhập
    public string characterSpriteLibraryAssetName; // Để lưu tên file SpriteLibraryAsset đã chọn
    public List<SeedSaveData> plantedCrops;

    //Constructor khi khogn6 có file save
    public GameData()
    {
       this.selectedCharacterName = "NoName";//tên mặc định
       this.characterSpriteLibraryAssetName = "DefaultSpriteLibrary";//tên file mặc định
    }
}
//lớp con lưu thông tin cây trồng
[System.Serializable]
public class SeedSaveData
{
    public string SceneName;//scene được trồng cây
    public SerializableVector3 worldPosition;//vị trí cây trồng trong thế giới
    public int cropDataID;//tên file CropData được tham chiếu
    public double timePlanted;//thời gian trồng cây (dùng để tính thời gian phát triển của cây)
}
[System.Serializable]
public class SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public Vector3 ToVector3() => new Vector3(x, y, z);
}
