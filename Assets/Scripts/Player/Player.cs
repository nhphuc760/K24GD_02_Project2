
using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerVisual playerVisual;
    public bool isInFishingState;
    public Transform fishingPoint;
    public Transform fishingPointBack;
    public Transform fishingPointFront;
    public Transform fishingPointLeft;
    public Transform fishingPointRight;
    public Transform bobber;
    public float targetTime = 0.0f;
    public float extraBobberDistance;
    public GameObject fishGame;
    public GameObject fish;
    private Tilemap[] _groundTilemaps;

    bool isFishing;
    public bool IsFishing { get => isFishing; set{
            if(isFishing == value) return;
            if (isFishing)
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
        DontDestroyOnLoad(this.gameObject);
        playerMovement ??= GetComponent<PlayerMovement>();
        playerVisual ??= GetComponentInChildren<PlayerVisual>();             
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fishGame.SetActive(false);
        targetTime = 0.0f;
        extraBobberDistance = 0.0f;
        SceneManager.sceneLoaded += OnSceneLoad;
        GameEventManager.Ins.onFishingZoneEnter += () => isInFishingZone = true;
        GameEventManager.Ins.onFishingZoneExit += () => isInFishingZone = false;
    }
    private void OnSceneLoad(Scene arg0, LoadSceneMode arg1)
    {
        if (!arg0.path.StartsWith("Assets/Scenes/ScenePlay")) return;    
            FindAnyObjectByType<CinemachineCamera>().Follow = this.transform;
            if (arg0.name == "BeachScene")
            {
                Tilemap[] allTilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
                _groundTilemaps = Array.FindAll(allTilemaps, tm => tm.gameObject.layer == LayerMask.NameToLayer("Ground"));
            }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
        GameEventManager.Ins.onFishingZoneEnter -= () => isInFishingZone = true;
        GameEventManager.Ins.onFishingZoneExit -= () => isInFishingZone = false;

    }
    private void Update()
    {
        if (!isInFishingZone) return;
        isBobberInWater = CheckBobberInWater(playerMovement.GetDirection());
        HandleInput();
        if (isInFishingState && !isFishing)
        {
            targetTime += Time.deltaTime;
        }
    }
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && playerVisual.winnerAnimIsActive == false && !fishGame.activeSelf && !fishBited && isBobberInWater)
        {
            RemovePreviousBobber();
            StartPoleBack();
        }
        if (Input.GetKeyUp(KeyCode.Space) && playerVisual.winnerAnimIsActive == false && isInFishingState && !fishGame.activeSelf && !fishBited)
        {
            CastingFishing();
        }
        if (Input.GetKeyDown(KeyCode.P) && isInFishingState)
        {
            RemovePreviousBobber();
            CancelFishing();
        }
    }

    private void StartPoleBack()
    {
        IsFishing = false;
        targetTime = 0.0f;
        isInFishingState = true;
        playerVisual.animator.Play(AnimationHashes.POLE_BACK);
    }
    private void CastingFishing()
    {
        playerVisual.animator.Play(AnimationHashes.CASTING_FISHING);
        if (targetTime >= 3)
        {
            extraBobberDistance += 3;
        }
        else
        {
            extraBobberDistance += targetTime;
        }
    }
    private void CancelFishing()
    {
        fishGame.SetActive(false);
        playerVisual.animator.Play(AnimationHashes.CAPTURE_NOFISH);
       
        IsFishing = false;
        isInFishingState = false;
        FishBited = false;
    }


    public void fishGameWon()
    {
        FishGameResult = true;
        playerVisual.animator.Play(AnimationHashes.CAPTURE_NOFISH);
        isInFishingState = false;
        fishGame.SetActive(false);   
        IsFishing = false;
        FishBited = false;
    }
    public void fishGameLossed()
    {
        FishGameResult = false;
        playerVisual.animator.Play(AnimationHashes.CAPTURE_NOFISH);
        isInFishingState = false;
        fishGame.SetActive(false);         
        IsFishing = false;
        FishBited = false;
    }
    public void OnCastFishingEnd()
    {
        UpdateFishingPointPosition();
        Vector3 temp = extraBobberDistance * (Vector3)playerMovement.GetDirection();
        fishingPoint.transform.position += temp;        
        IsFishing = true;
        bobber.gameObject.SetActive(true);
        bobber.position = fishingPoint.position;
        fishingPoint.transform.position -= temp;
        targetTime = 0.0f;
        extraBobberDistance = 0.0f;
    }
    private void RemovePreviousBobber()
    {
        bobber.gameObject.SetActive(false);
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
