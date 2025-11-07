using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image fill;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI percentageTxt;
    [Header("Config")]
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float speedFade = 0.3f;
    public static LoadingScene Ins;
    public bool isLoading;
    private void Awake()
    {
      if(Ins != null &&  Ins != this)
        {
            Destroy(Ins.gameObject);
        }   
      Ins = this;
      DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        HideInstant();
    }

    public void LoadScene(string oldSceneName, string newSceneName, string description, LoadSceneMode loadSceneMode)
    {
        Show();
        StartCoroutine(LoadSceneAsync(oldSceneName, newSceneName, description, loadSceneMode));
    }

    public void LoadScene(int oldIndex, int newIndex, string description, LoadSceneMode loadSceneMode)
    {
        Show();
        StartCoroutine(LoadSceneAsync(oldIndex, newIndex, description, loadSceneMode));
    }

    IEnumerator LoadSceneAsync(int oldIndex, int newIndex, string description, LoadSceneMode loadMode)
    {
        isLoading = true;
        var task = SceneManager.LoadSceneAsync(newIndex, loadMode);
        float progress = 0f;
        task.allowSceneActivation = false;
        while (!task.isDone)
        {
            float target = Mathf.Clamp01(task.progress / 0.9f);
            progress = Mathf.MoveTowards(progress, target, Time.deltaTime * 9f);
            fill.fillAmount = progress;
            percentageTxt.text = $"{(progress * 100).ToString("F1")}%";
            if (progress >= 1f)
            {
                yield return new WaitForSeconds(.2f);
                task.allowSceneActivation = true;
            }
            yield return null;

        }
        if (loadMode.Equals(LoadSceneMode.Additive))
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(newIndex));
            SceneManager.UnloadSceneAsync(oldIndex);
        }

        Hide();
        isLoading = false;

    }
    IEnumerator LoadSceneAsync(string oldSceneName, string newSceneName, string description, LoadSceneMode loadMode)
    {
       
        isLoading = true;
        var task = SceneManager.LoadSceneAsync(newSceneName, loadMode);
        float progress = 0f;
        task.allowSceneActivation = false;
        while (!task.isDone)
        {
            float target = Mathf.Clamp01( task.progress/0.9f);
            progress = Mathf.MoveTowards(progress, target, Time.deltaTime * 9f);
            fill.fillAmount = progress;
            percentageTxt.text = $"{(progress *100).ToString("F1")}%";
            if (progress >= 1f)
            {
                yield return new WaitForSeconds(.2f);
                task.allowSceneActivation=true;
            }
            yield return null;

        }

        if (loadMode.Equals(LoadSceneMode.Additive))
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(newSceneName));
            SceneManager.UnloadSceneAsync(oldSceneName);
        }
        Hide();
        isLoading=false;

    }

    void Show()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    void Hide()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        StartCoroutine(FadeOut());
    }

    void HideInstant()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    IEnumerator FadeOut()
    {
        float t = 0;
        while (t < speedFade)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1 - t / speedFade;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
