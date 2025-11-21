// Thêm class này vào file GameData.cs (hoặc file riêng)
using System;
using Newtonsoft.Json;

[System.Serializable]
public class AnimalSaveData
{
    public SerializableVector3 worldPosition;
    public string animalDataID; // Sẽ lưu AnimalData._id
    public DateTime timeProductReady; // Lưu thời điểm sản phẩm chín
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}