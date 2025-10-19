using System.Collections.Generic;
using UnityEngine;


//lớp chính chứa toàn bộ dữ liệu gane
[System.Serializable]
public class GameData
{
    public string selectedCharacterName;          // Để lưu tên người chơi đã nhập
    public string characterSpriteLibraryAssetName; // Để lưu tên file SpriteLibraryAsset đã chọn
    public List<CropSaveData> plantedCrops =new List<CropSaveData>();//danh sách cây trồng đã lưu

    //Constructor khi khogn6 có file save
    public GameData()
    {
       this.plantedCrops = new List<CropSaveData>();
       this.selectedCharacterName = "Player";//tên mặc định
       this.characterSpriteLibraryAssetName = "DefaultSpriteLibrary";//tên file mặc định
    }
}
//lớp con lưu thông tin cây trồng
[System.Serializable]
public class CropSaveData
{
    public string SceneName;//scene được trồng cây
    public Vector3 worldPosition;//vị trí cây trồng trong thế giới
    public string cropDataID;//tên file CropData được tham chiếu
    public double timePlanted;//thời gian trồng cây (dùng để tính thời gian phát triển của cây)
}