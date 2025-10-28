using UnityEngine;

public static class GameLoader
{
    // Biến để giữ dữ liệu được tải lên TẠM THỜI
    public static GameData LoadedData { get; private set; }
    // Cờ để báo cho GameManager biết có phải đang tải game không
    public static bool IsLoadingGame { get; private set; }

    // Hàm này sẽ được gọi bởi Main Menu khi nhấn "Continue"
    public static void LoadGame(GameData data)
    {
        LoadedData = data;
        IsLoadingGame = true;
    }

    // Hàm này được gọi khi bắt đầu Game Mới hoặc sau khi đã tải xong phần tạo nhân vật
    public static void NewGame()
    {
        LoadedData = null;
        IsLoadingGame = false;
    }
}
