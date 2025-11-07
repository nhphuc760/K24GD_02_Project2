using UnityEngine;
public class SeedSaveData
{
    public string SceneName;//scene được trồng cây
    public SerializableVector3 worldPosition;//vị trí cây trồng trong thế giới
    public int cropDataID;//tên file CropData được tham chiếu
    public double timePlanted;//thời gian trồng cây (dùng để tính thời gian phát triển của cây)
}
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
