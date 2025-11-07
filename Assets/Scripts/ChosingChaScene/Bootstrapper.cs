// Bootstrapper.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    //scene đầu tiên  muốn người chơi thấy
    [SerializeField] string nextScene = "Farm";
    void Start()
    {
        StartCoroutine(WaitLoadingScene());
    }

    IEnumerator WaitLoadingScene()
    {
        yield return null;
        while (LoadingScene.Ins.isLoading)
        {
            yield return null;  
        }
        LoadingScene.Ins.LoadScene(SceneManager.GetActiveScene().name, nextScene, "Dữ liệu đã sẵn sàng", LoadSceneMode.Additive);
    
    }
}