using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button loadGameButton;

    public string newGameScene = "CustomizeCharacter";

    public string mainGameScene = "Farm";

    private void Start()
    {
        //kiểm tra có file save hay không để kích hoat nút Load Game
        if(loadGameButton != null)
        {
            loadGameButton.interactable = SaveSystem.DoesSaveFileExist();
        }
    }

    //hàm được gòi khi nhấn nút New Game
    public void OnNewGameButton()
    {
        GameLoader.NewGame();//đặt cờ không tải game
        SceneManager.LoadScene(newGameScene);//chuyển đến
    }

    public void OnLoadGameButton()
    {
        if (SaveSystem.DoesSaveFileExist())
        {
            GameData data = SaveSystem.LoadGame();//tải dữ liệu từ file
            GameLoader.LoadGame(data);//đặt cờ tải game và lưu dữ liệu tạm thời
            SceneManager.LoadScene(mainGameScene);//chuyển đến cảnh chính của game
        }
        else
        {
            Debug.LogWarning("No save file found. Cannot load game.");
        }
    }
}
