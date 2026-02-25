// Bootstrapper.cs
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    //scene đầu tiên  muốn người chơi thấy
    [SerializeField] string nextScene = "Farm";
    async void Start()
    {
        await WaitLoadingScene();
    }

    async UniTask WaitLoadingScene()
    {
        await UniTask.Yield();
        if(LoadingScene.Ins == null)
        {
            SceneManager.LoadScene(nextScene);
           await UniTask.Yield();
            return;
        }
        while (LoadingScene.Ins.IsBusy)
        {
            await UniTask.Yield();
        }
        
        await LoadingScene.Ins.LoadScene( nextScene, "Dữ liệu đã sẵn sàng", LoadSceneMode.Single, true);

    }
}