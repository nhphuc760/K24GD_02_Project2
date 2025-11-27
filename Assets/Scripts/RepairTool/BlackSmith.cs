using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
[RequireComponent(typeof(Collider2D))]
public class BlackSmith : MonoBehaviour
{
    [SerializeField] DropSlotUI dropSlotUI;
    [SerializeField] GameObject canvas;
    [SerializeField] Transform container;
    InventoryManager invenManager;
    List<InventorySlotUI> cachedSlotUI = new List<InventorySlotUI>();
    [SerializeField] Transform parentIcon;
    [SerializeField] TextMeshProUGUI coinText;
    bool isTraing;
    [Header("DisplayTrain")]
    [SerializeField] Image _icon;
    [SerializeField] Image fill;
    [SerializeField] TextMeshProUGUI coolDownText;
    [SerializeField] Animator animator;
    [SerializeField] RectTransform UITrain;
    [SerializeField] GameObject containerFill_Text;
    public bool IsTraing { get => isTraing; set => isTraing = value; }
    DateTime coolDownStart;
    DateTime coolDownEnd;
    BlackSmithTrainData saveData;
    float totalTime;
    Vector2 originAnchorUITrain;

    public AudioClip TrainingSound;
    private AudioSource audioSource;
    private async void Awake()
    {
        originAnchorUITrain = UITrain.position;
        var task = await Save_Load_Firebase.LoadData("BlackSmith");
        if (task != null && task.Exists)
        {
            string value = task.Value.ToString();
            if (string.IsNullOrEmpty(value)) return;
            saveData = JsonConvert.DeserializeObject<BlackSmithTrainData>(value);
            if (saveData == null) return;
            var curTime = await Save_Load_Firebase.GetSeverDateTime();
            if(curTime != null && curTime.HasValue)
            {
                coolDownStart = curTime.Value;
            }
            else
            {
                coolDownStart = DateTime.UtcNow;
            }
            coolDownEnd = saveData.endTraining;
            
            totalTime = saveData.totalTime;
            TimeSpan timeSpand = coolDownEnd - coolDownStart;
            if (timeSpand > TimeSpan.Zero)
            {
                ShowUITrain();
                StartCoroutine(Train());
            }
            else
            {
                isTraing = false;
                ShowUIComplete();
            }
                ItemDataSO tool = GameEventManager.Ins.inventoryEvent.GetItemSOByID(saveData._idTool);
            _icon.sprite = tool._icon;

        }
        else
        {
            HideUITrain();
        }
    }

   

    private async void OnTriggerEnter2D(Collider2D collision)
    {
         if(collision.CompareTag("GroundCheck"))
         {
            if (isTraing) return;
            if(!isTraing && saveData != null)
            {
               // rèn xong
               HideUITrain();
                UIManager.instance.Hide();
                GameEventManager.Ins.gameInput.Disable_InputAction();
               ToolRunTimeData tool = new ToolRunTimeData();
                tool.currentDurability = saveData.curDurability + saveData.spandDurability;
                ItemDataSO item = GameEventManager.Ins.inventoryEvent.GetItemSOByID(saveData._idTool);               
                await GetItemEffect().ToUniTask();
                GameEventManager.Ins.inventoryEvent.AddItem(item, 1, tool, isDialog: false);
                GameEventManager.Ins.OnRepairTool(item as ToolDataSO);
                saveData = null;
            }
            if(invenManager == null){
                invenManager = GameEventManager.Ins.inventoryEvent.GetDataInventory();
                Debug.Log("Has Inventory: " + invenManager != null);
                if(invenManager == null) return;
               
                    InitSlot();
            }
            ShowCanvas();
        }
    }

    IEnumerator GetItemEffect()
    {
        GameObject getItemIcon = new GameObject("Clone");
        getItemIcon.transform.localScale = new Vector3(.8f, .8f, 0f);
        getItemIcon.AddComponent<SpriteRenderer>().sprite = _icon.sprite;
        getItemIcon.transform.position = this.transform.position + Vector3.up * .5f;
        float timeDelay = 1.5f;
        float speedUp = 1.2f;
        while (timeDelay > 0)
        {
            timeDelay -= Time.deltaTime;
            getItemIcon.transform.position += Vector3.up * Time.deltaTime * speedUp;
            yield return null;
        }
        Destroy(getItemIcon);
    }

    void InitSlot()
    {
        for (int i = 0; i < invenManager.slotAmount; i++)
        {
            var slotUI = Instantiate(invenManager.slotUIPrefab, container);
            slotUI.Init(invenManager, i);
            slotUI.SetIconParent(parentIcon);
            cachedSlotUI.Add(slotUI);
        }
    }

    public void ShowCanvas()
    {
        coinText.text = GameManager.Ins.Coin.ToString();
        invenManager.SetCachedSlotUI(cachedSlotUI);
        invenManager.UpdateUI();
        canvas.SetActive(true);
        GameEventManager.Ins.gameInput.Disable_InputAction();
        UIManager.instance.Hide();
    }
    public void HideCanvas()
    {
        invenManager.RestoreDataBackup();
        canvas.SetActive(false);
        GameEventManager.Ins.gameInput.Enable_InputAction();
        UIManager.instance.Show();
    }
    public bool CheckCoin(int input)
    {
        if (GameManager.Ins.Coin >= input)
        {
            return true;
        }
        else
        {
            StartCoroutine(ShakeCoin());
            return false;
        }


    }
    IEnumerator ShakeCoin()
    {
        Vector3 origin = coinText.rectTransform.anchoredPosition;
        float timeShake = .3f;
        float magnitude = 10f;
        float timeCount = 0;
        while (timeCount < timeShake)
        {
            
            coinText.color = Color.red;
           Vector3 offset = UnityEngine.Random.insideUnitCircle * magnitude;
            coinText.rectTransform.anchoredPosition = origin + offset;
            timeCount += Time.deltaTime;
            yield return null;
        }
        coinText.color = Color.white;
        coinText.rectTransform.anchoredPosition = origin;
    }

    public async Task StartTrain(float time, Sprite icon)
    {
         saveData = dropSlotUI.GetDataTool();
        var task = await Save_Load_Firebase.GetSeverDateTime();
        if(task != null && task.HasValue)
        {
            coolDownEnd = task.Value.AddSeconds(time);
            coolDownStart = task.Value;
            saveData.endTraining = coolDownEnd;
        }
        else
        {
            coolDownStart = DateTime.UtcNow;
            coolDownEnd = coolDownStart.AddSeconds(time);
            saveData.endTraining = coolDownEnd;
        }
        totalTime = time;
        await Save_Load_Firebase.SaveData("BlackSmith", saveData.SerilizeObject());
        _icon.sprite = icon;
        StartCoroutine(Train());
    }

    IEnumerator Train()

    { 
        isTraing = true;
        animator.Play("Train");
        ShowUITrain();
        while (coolDownStart < coolDownEnd)
        {
            TimeSpan timeSpan = coolDownEnd- coolDownStart;
            float timeRemaining = (float)timeSpan.TotalSeconds;
            float fillValue = 1 - timeRemaining/totalTime;
            fill.fillAmount = fillValue;
            fill.color = Color.Lerp(Color.red, Color.green, fillValue);
            coolDownText.text = timeSpan.ToString(@"hh\:mm\:ss");
            coolDownStart = coolDownStart.AddSeconds(1f);
            yield return new WaitForSeconds(1f);
        }
        isTraing = false;
        ShowUIComplete();
        animator.Play("Idle_Front");
        yield return null;
    }
    public void PlaySoundEffect()
    {
        //Trigger sound here
        Debug.Log("PlaySoundEffect called");
        if (audioSource != null)
        {
            AudioManager.instance.PlayFX(TrainingSound);
        }
    }
    private async void OnDestroy()
    {
        if(saveData != null){
        saveData.isTraing = this.isTraing;
            await Save_Load_Firebase.SaveData("BlackSmith", saveData.SerilizeObject());
        }
        else
        {
            await Save_Load_Firebase.RemoveAsync("BlackSmith");
        }
    }

    void ShowUITrain()
    {
        UITrain.position = originAnchorUITrain;
        
        _icon.enabled = true;
        containerFill_Text.SetActive(true);
    }
    void ShowUIComplete()
    {
        UITrain.position += new Vector3(1.4f, 0, 0);
        _icon.enabled = true;
        containerFill_Text.SetActive(false);
    }
    void HideUITrain()
    {
        _icon.enabled = false;
        containerFill_Text.SetActive(false);
    }

    
}

public class BlackSmithTrainData
{
    public bool isTraing;
    public int _idTool;
    public string _dataRuntime;
    public int spandDurability;
    public int curDurability;
    public DateTime endTraining;
    public float totalTime;
    public string SerilizeObject()
    {
        return JsonConvert.SerializeObject(this);
    }
}



