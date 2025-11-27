using System.Threading.Tasks;
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

    public async void OnMainMenuPressed()
    {
        if (GameEventManager.Ins != null && UIManager.instance != null){
            Destroy(GameEventManager.Ins.gameObject);
            GameEventManager.Ins = null;
            UIManager.instance = null;
        }
   
        if(GameManager.Ins != null)
        {
            Destroy(GameManager.Ins.gameObject);
            GameManager.Ins = null;
        }
        if(Player.Instance != null)
        {
            Destroy(Player.Instance.gameObject);
            Player.Instance = null; 
        }
        if (AudioManager.instance != null)
        {
            Destroy (AudioManager.instance.gameObject);
            AudioManager.instance = null;
        }
        await LoadingScene.Ins.LoadScene(mainMenuScene, "Loading...", LoadSceneMode.Single);
    }

    //Exit Game
    public  void OnExitPressed()
    {
       
        Save_Load_Firebase.LogOut();
        Application.Quit();
      
    }
}