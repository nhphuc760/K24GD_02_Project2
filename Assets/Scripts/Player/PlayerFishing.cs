
using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Tilemaps;

public class PlayerFishing : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerVisual playerVisual;
    [SerializeField] GameObject fishIcon;
    public bool isInFishingState;
    public Transform fishingPoint;
    public Transform fishingPointBack;
    public Transform fishingPointFront;
    public Transform fishingPointLeft;
    public Transform fishingPointRight;
    public Transform bobberPoint;
    public GameObject fishGame;
    public GameObject fish;
    private Tilemap[] _groundTilemaps;
    InventorySlot curToolKit;
    ToolDataSO curToolDataSO;
    BaitDataSO bait;
    [SerializeField] FishDataBase fishDatabase;

    public AudioClip ThrowRob;
    public AudioClip ThrowRobBack;
    public bool IsFishing { get => isInFishingState; set{
            if(isInFishingState == value) return;
            isInFishingState = value;
            if (value)
            {
                GameEventManager.Ins.gameInput.Disable_InputAction();
            }
            else
            {
                GameEventManager.Ins.gameInput.Enable_InputAction();
            }
        } 
    }


    private bool fishGameResult = false;
    public bool FishGameResult { get => fishGameResult; set => fishGameResult = value; }

    private bool fishBited = false;
    public bool FishBited { get => fishBited; set => fishBited = value; }

    private bool isBobberInWater = false;
    public bool IsBobberInWater { get => isBobberInWater; set => isBobberInWater = value; }

     bool isInFishingZone;

    private void Awake()
    {
        playerMovement ??= GetComponent<PlayerMovement>();
        playerVisual ??= GetComponentInChildren<PlayerVisual>();
        GameEventManager.Ins.toolKitEvent.onCurSelectedChange += CurSelectToolKitChange;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void CurSelectToolKitChange(InventorySlot inventorySlot)
    {
        curToolKit = inventorySlot;
        curToolDataSO =  curToolKit.ItemData as ToolDataSO;
    }
    void Start()
    {
        fishGame.SetActive(false);
        SceneManager.sceneLoaded += OnSceneLoad;
        GameEventManager.Ins.onFishingZoneEnter += FishingZoneEnter;
        GameEventManager.Ins.onFishingZoneExit += FishingZoneExit;
    }


    void FishingZoneEnter()
    {
        Debug.Log("Enter Fishing Zone");
        isInFishingZone = true;
    }
    void FishingZoneExit()
    {
        Debug.Log("Exit Fishing Zone");
        isInFishingZone = false;
    }

    private void OnSceneLoad(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "BeachScene")              
        {
            this.enabled = true;
            Tilemap[] allTilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
            _groundTilemaps = Array.FindAll(allTilemaps, tm => tm.gameObject.layer == LayerMask.NameToLayer("Ground"));
        }
        else
        {
            this.enabled = false;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
        if (GameEventManager.Ins != null)
        {
            GameEventManager.Ins.onFishingZoneEnter -= FishingZoneEnter;
            GameEventManager.Ins.onFishingZoneExit -= FishingZoneExit;
            GameEventManager.Ins.toolKitEvent.onCurSelectedChange -= CurSelectToolKitChange;
        }

    }
    private void Update()
    {
        if (!isInFishingZone) return;
        
        if (curToolDataSO == null || curToolDataSO.toolType != ToolDataSO.ToolType.FishingRod)
        {
            return;
        }
        isBobberInWater = CheckBobberInWater(playerMovement.GetDirection());
        HandleInput();
    }
    ToolRunTimeData fishingRodData;
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && playerVisual.winnerAnimIsActive == false && !fishGame.activeSelf && !fishBited && isBobberInWater)
        {
            if(AudioManager.instance != null)
            {
                Debug.Log("Play Throw Rob Back Sound");
                AudioManager.instance.PlayFX(ThrowRobBack);
            }
            RemovePreviousBobber();
            StartPoleBack();
        }
        if (Input.GetKeyUp(KeyCode.Space) && playerVisual.winnerAnimIsActive == false && isInFishingState && !fishGame.activeSelf && !fishBited)
        {
             fishingRodData = curToolKit.dataRuntime as ToolRunTimeData;
            if(fishingRodData.currentDurability < curToolDataSO.durabilityLossPerUse)
            {
                GameEventManager.Ins.TriggerDialog("Cần câu đã hư hại, bạn cần gặp BlackSmith để sửa chữa");
                CancelFishing();
                return;
            }
            bait = GameEventManager.Ins.inventoryEvent.GetBaitDataSO();
            if(bait == null)
            {
                GameEventManager.Ins.TriggerDialog("<color=red>Không có mồi câu cá</color>");
                CancelFishing();
                return;
            }

            CastingFishing();
            if(AudioManager.instance != null)
            {
                Debug.Log("Play Throw Rob Sound");
                if (ThrowRob == null)
                {
                    Debug.LogError("LỖI: Chưa gán file âm thanh ThrowRob vào Inspector của PlayerFishing!");
                }
                else
                {
                    AudioManager.instance.PlayFX(ThrowRob);
                }
                // -----
            }
        }
        if (Input.GetKeyDown(KeyCode.P) && isInFishingState)
        {
            RemovePreviousBobber();
            CancelFishing();
        } 

    }

    private void StartPoleBack()
    {
      
        IsFishing = true;
        playerVisual.animator.Play(AnimationHashes.POLE_BACK);
    }
    private void CastingFishing()
    {
        playerVisual.animator.Play(AnimationHashes.CASTING_FISHING);
    }
    private void CancelFishing()
    {
        fishGame.SetActive(false);
        playerVisual.animator.Play(AnimationHashes.CAPTURE_NOFISH);
        IsFishing = false;
        FishBited = false;
    }


    public void fishGameWon()
    {
        if (AudioManager.instance != null)
        {
            Debug.Log("Play Throw Rob Sound");
            if (ThrowRob == null)
            {
                Debug.LogError("LỖI: Chưa gán file âm thanh ThrowRob vào Inspector của PlayerFishing!");
            }
            else
            {
                AudioManager.instance.PlayFX(ThrowRob);
            }
        }
        FishGameResult = true;
        playerVisual.animator.Play(AnimationHashes.CAPTURE_NOFISH);
        fishGame.SetActive(false);   
        IsFishing = false;
        FishBited = false;
        fishIcon.SetActive(false);
        FishDataSO sO = fishDatabase.GetRandomFish();
        fishingRodData.currentDurability -= curToolDataSO.durabilityLossPerUse;
        GameEventManager.Ins.inventoryEvent.AddItem(sO, 1, isDialog: false);
        GameEventManager.Ins.TriggerDialog($"<color=green>Bạn vừa câu được cá {sO._itemName}</color>");
        GameEventManager.Ins.OnFishing(sO);
        GameEventManager.Ins.inventoryEvent.RemoveItemByData(bait, 1);
        StaminaManager.instance.DecreaseStamina(curToolDataSO.decreaseStaminaPerUse);
    }
    public void fishGameLossed()
    {
        if (AudioManager.instance != null)
        {
            Debug.Log("Play Throw Rob Sound");
            if (ThrowRob == null)
            {
                Debug.LogError("LỖI: Chưa gán file âm thanh ThrowRob vào Inspector của PlayerFishing!");
            }
            else
            {
                AudioManager.instance.PlayFX(ThrowRob);
            }
        }
        FishGameResult = false;
        playerVisual.animator.Play(AnimationHashes.CAPTURE_NOFISH);
        fishGame.SetActive(false);         
        IsFishing = false;
        FishBited = false;
        fishIcon.SetActive(false);
        fishingRodData.currentDurability -= curToolDataSO.durabilityLossPerUse;
        GameEventManager.Ins.TriggerDialog("<color=red>Trật mất rồi...huhu</color>");
        GameEventManager.Ins.inventoryEvent.RemoveItemByData(bait, 1);
        StaminaManager.instance.DecreaseStamina(curToolDataSO.decreaseStaminaPerUse);
    }
    public void OnCastFishingEnd()
    {
        UpdateFishingPointPosition();      
        bobberPoint.gameObject.SetActive(value: true);
        bobberPoint.position = fishingPoint.position;

        if (AudioManager.instance != null)
        {
            Debug.Log("Play Throw Rob Sound");
            AudioManager.instance.PlayFX(ThrowRob);
        }
    }
    private void RemovePreviousBobber()
    {
        bobberPoint.gameObject.SetActive(false);
    }
    private bool CheckBobberInWater(Vector2 direction)
    {
        UpdateFishingPointPosition();
        Vector3Int cellPos;
        foreach (Tilemap tilemap in _groundTilemaps)
        {
            cellPos = tilemap.WorldToCell(fishingPoint.position);
            if (tilemap.GetTile(cellPos) != null)
            {
                return false;
            }
        }
        return true;
    }
    private void UpdateFishingPointPosition()
    {
        Vector2 dir = playerMovement.GetDirection();
        if (dir == Vector2.zero) return;

        // Các hướng chuẩn
        Vector2[] dirs = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        // Tương ứng là vị trí cần đặt
        Transform[] points =
        {
        fishingPointBack,
        fishingPointFront,
        fishingPointLeft,
        fishingPointRight
    };

        float bestDot = float.NegativeInfinity;
        Transform bestPoint = fishingPointFront;

        for (int i = 0; i < dirs.Length; i++)
        {
            float dot = Vector2.Dot(dir, dirs[i]);
            if (dot > bestDot)
            {
                bestDot = dot;
                bestPoint = points[i];
            }
        }

        fishingPoint.position = bestPoint.position;
    }

    public IEnumerator StartMiniGameAfterAlertTime(float AlertFishBiteTime)
    {
        yield return new WaitForSeconds(AlertFishBiteTime);
        playerVisual.animator.Play(AnimationHashes.ROLL);
        fishGame.SetActive(true);
    }
    public void CheckIfWinFishGame()
    {
        if (FishGameResult)
        {
            FishGameResult = false;
            fish.SetActive(true);
        }
        else
            fish.SetActive(false);
    }
    
    
    
}
