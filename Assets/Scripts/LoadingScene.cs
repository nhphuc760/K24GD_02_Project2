using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

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
    public bool isFading;
    public bool IsBusy => isLoading || isFading;


    private void Awake()
    {
      if(Ins != null &&  Ins != this)
        {
            Destroy(Ins.gameObject);
            return;
        }   
      Ins = this;
      DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        HideInstant();
    }

    public async UniTask LoadScene( string newSceneName, string description, LoadSceneMode loadSceneMode, bool fadeOut = true, float speedFade = 0.3f)
    {
        Show();
        this.speedFade = speedFade;
        //StartCoroutine(LoadSceneAsync(newSceneName, description, loadSceneMode, fadeOut));
        await LoadSceneAsync(newSceneName, description, loadSceneMode, fadeOut).ToUniTask();
    }

    public async UniTask LoadScene( int newIndex, string description, LoadSceneMode loadSceneMode, bool fadeOut = true, float speedFade = 0.3f)
    {
        Show();
        this.speedFade = speedFade;
        //StartCoroutine(LoadSceneAsync( newIndex, description, loadSceneMode, fadeOut));
        await LoadSceneAsync(newIndex, description, loadSceneMode, fadeOut).ToUniTask();
    }

    IEnumerator LoadSceneAsync( int newIndex, string description, LoadSceneMode loadMode, bool fadeOut)
    {
        isLoading = true;
        var task = SceneManager.LoadSceneAsync(newIndex, loadMode);
        float progress = 0f;
        task.allowSceneActivation = false;
        this.description.text = description;
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
        isLoading = false;
        Hide(fadeOut);

    }
    IEnumerator LoadSceneAsync( string newSceneName, string description, LoadSceneMode loadMode, bool fadeOut)
    {
       
        isLoading = true;
        var task = SceneManager.LoadSceneAsync(newSceneName, loadMode);
        float progress = 0f;
        task.allowSceneActivation = false;
        this.description.text = description;
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

        isLoading = false;
        Hide(fadeOut);
       

    }


    private void OnEnable()
    {
        if(GameEventManager.Ins != null)
        {
            GameEventManager.Ins.gameInput.Disable_InputAction();
        }
    }
    private void OnDisable()
    {
        if (GameEventManager.Ins != null)
        {
            GameEventManager.Ins.gameInput.Enable_InputAction();
        }
    }

    void Show()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;


    }

    void Hide(bool fading)
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        if(fading)
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
        isFading = true;
        float t = 0;
        while (t < speedFade)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1 - t / speedFade;
            yield return null;
        }
       isFading = false;
        gameObject.SetActive(false);
    }
}
