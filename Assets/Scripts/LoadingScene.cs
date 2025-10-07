using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] Image fill;
    [SerializeField] float timeLoad = 3f;
    void Start()
    {
        StartCoroutine(Loading());
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
        SceneManager.LoadScene("PlayScene");
    }

    
}
