using System.Threading.Tasks;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button loadGameButton;

    public string newGameScene = "CustomizeCharacter";

    public string mainGameScene = "Farm";
    bool isDataExists = false;
    private async void Start()
    {
        var task = await Save_Load_Firebase.LoadGame();
        isDataExists = task != null;
        //kiểm tra có file save hay không để kích hoat nút Load Game
        if (loadGameButton != null)
        {
          

            loadGameButton.interactable = isDataExists;
        }
    }

    //hàm được gòi khi nhấn nút New Game
    public void OnNewGameButton()
    {
        GameLoader.NewGame();//đặt cờ không tải game
        SceneManager.LoadScene(newGameScene);//chuyển đến
    }

    public async void OnLoadGameButton()
    {
        if (isDataExists)
        {
            GameData data = await Save_Load_Firebase.LoadGame();//tải dữ liệu từ file
            GameLoader.LoadGame(data);//đặt cờ tải game và lưu dữ liệu tạm thời
            SceneManager.LoadScene(mainGameScene);//chuyển đến cảnh chính của game
        }
        else
        {
            Debug.LogWarning("No save file found. Cannot load game.");
        }
    }
}
