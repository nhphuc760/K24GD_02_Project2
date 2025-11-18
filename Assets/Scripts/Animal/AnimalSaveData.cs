// Thêm class này vào file GameData.cs (hoặc file riêng)
using System;

[System.Serializable]
public class AnimalSaveData
{
    public string sceneName;
    public SerializableVector3 worldPosition;
    public string animalDataID; // Sẽ lưu AnimalData._id
    public DateTime timeProductReady; // Lưu thời điểm sản phẩm chín
}