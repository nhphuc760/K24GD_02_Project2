using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] Image fill;
    [SerializeField] float timeLoad = 3f;
    public static string sceneTarget;
    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    // Update is called once per frame
   IEnumerator Loading()
    {

        float timeCoolDown = timeLoad;
        while (timeCoolDown > 0f)
        {
            timeCoolDown -= Time.deltaTime;
            fill.fillAmount =1f - (timeCoolDown/timeLoad);
            yield return null;
        }
        SceneManager.LoadScene("WaterFall");
    }
    
    IEnumerator LoadSceneAsync()
    {
        var task = SceneManager.LoadSceneAsync(sceneTarget);
        while (!task.isDone)
        {
            fill.fillAmount = Mathf.Clamp01( task.progress/0.9f);
            yield return null;
        }
    }  
}
