using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button loadGameButton;

    [SerializeField] string newGameScene;
    [SerializeField] string loadGameScene;
    bool isDataExists = false;
    private async void Start()
    {
       try {
        var task = await Save_Load_Firebase.LoadData("PlayerData");
        isDataExists = task != null;
        //kiểm tra có file save hay không để kích hoat nút Load Game
        if (loadGameButton != null)
        {
            loadGameButton.interactable = isDataExists;
        }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            loadGameButton.interactable = false;
        }
    }

    //hàm được gòi khi nhấn nút New Game
    public void OnNewGameButton()
    {
        LoadingScene.Ins.LoadScene( newGameScene, "Loading...", LoadSceneMode.Single, true);
       
    }

    public void OnLoadGameButton()
    {
        if (isDataExists)
        {
            LoadingScene.Ins.LoadScene( loadGameScene, "Đang tải dữ liệu người chơi", LoadSceneMode.Single, false);
        }
    }
}
