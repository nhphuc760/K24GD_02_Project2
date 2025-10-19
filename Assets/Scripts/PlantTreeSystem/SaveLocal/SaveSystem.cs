using System.IO;
using UnityEngine;

public class SaveSystem
{
    private static string saveFileName = "savegame.json";

    //hàm lưu dữ liệu game vào file
    public static void SaveGame(GameData data)
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        string json = JsonUtility.ToJson(data, true);//chuyển data thành json
        try 
        {
            File.WriteAllText(path, json);//ghi text vào file
            Debug.Log("Game saved to " + path);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save game: " + e.Message);
        }
    }



    public static GameData LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        if(File.Exists(path))//kiểm tra file có tồn tại hay không
        {
            try
            {
                string json = File.ReadAllText(path);//đọc text từ file
                GameData data = JsonUtility.FromJson<GameData>(json);//chuyển text thành data
                Debug.Log("Game loaded from " + path);
                return data;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to load game: " + e.Message);
                return new GameData();//trả về data mới nếu lỗi
            }
        }
        else
        {
            //nếu file không tồn tại
            Debug.Log("No save file found at " + path);
            return new GameData();//trả về data mới
        }
    }
}
