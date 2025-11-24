using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuController : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject menuPanel; 

    [Header("Scene Names")]
    public string mainMenuScene = "MainMenu";

    void Start()
    {
        // Luôn ẩn menu khi game bắt đầu
        if (menuPanel != null) menuPanel.SetActive(false);
    }


    //Mở Menu
    public void OnHomePressed()
    {
        menuPanel.SetActive(true);
    }

    //Đóng Menu
    public void OnClosePressed()
    {
        menuPanel.SetActive(false);
    }

    public void OnMainMenuPressed()
    {
        //// (Tùy chọn) Lưu game trước khi thoát
        //if (GameManager.instance != null)
        //{
        //    // GameManager.instance.SaveGame(); 
        //}
        SceneManager.LoadScene(mainMenuScene);
    }

    //Exit Game
    public void OnExitPressed()
    {
        //// (Tùy chọn) Lưu game trước khi thoát
        //if (GameManager.instance != null)
        //{
        //    // GameManager.instance.SaveGame(); 
        //}

        Application.Quit();
    }
}