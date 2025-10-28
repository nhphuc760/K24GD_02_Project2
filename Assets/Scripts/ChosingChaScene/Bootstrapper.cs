// Bootstrapper.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    //scene đầu tiên  muốn người chơi thấy
    public string startingSceneName = "CustomizeCharacter";
    // (Hoặc "MainMenu" nếu bạn có)

    void Start()
    {
        SceneManager.LoadScene(startingSceneName);
    }
}