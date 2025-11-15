
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
    public bool isFishing;
    public Transform fishingPoint;
    public Transform fishingPointBack;
    public Transform fishingPointFront;
    public Transform fishingPointLeft;
    public Transform fishingPointRight;
    public GameObject bobber;
    private Vector3 temp;
    public float targetTime = 0.0f;
    public float extraBobberDistance;
    public GameObject fishGame;
    public GameObject fish;
    public float timeTillCatch = 0.0f;
    private Tilemap[] _groundTilemaps;
   

    private bool fishGameResult = false;
    public bool FishGameResult { get => fishGameResult; set => fishGameResult = value; }

    private bool fishBited = false;
    public bool FishBited { get => fishBited; set => fishBited = value; }

    private bool isBobberInWater = false;
    public bool IsBobberInWater { get => isBobberInWater; set => isBobberInWater = value; }

    public bool IsInFishingZone { get => FishingZone.Ins.playerIsInFishingZone; }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if(playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement>();
        }
        if(playerVisual == null)
        {
            playerVisual = GetComponentInChildren<PlayerVisual>();
        }
        Tilemap[] allTilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        _groundTilemaps = Array.FindAll(allTilemaps, tm => tm.gameObject.layer == LayerMask.NameToLayer("Ground"));
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isFishing = false;
        fishGame.SetActive(false);
        targetTime = 0.0f;
        extraBobberDistance = 0.0f;
            SceneManager.sceneLoaded += OnSceneLoad;
        }

    private void OnSceneLoad(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.path.StartsWith("Assets/Scenes/ScenePlay"))
        {
            FindAnyObjectByType<CinemachineCamera>().Follow = this.transform;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
        isBobberInWater = CheckBobberInWater(new Vector2(playerMovement.HorizontalMovement, playerMovement.VerticalMovement));
        HandleInput();
        if (isInFishingState && !isFishing)
        {
            targetTime += Time.deltaTime;
        }
    }
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && playerVisual.winnerAnimIsActive == false && IsInFishingZone && !fishGame.activeSelf && !FishBited && isBobberInWater)
        {
            RemovePreviousBobber();
            StartPoleBack();
        }
        if (Input.GetKeyUp(KeyCode.Space) && playerVisual.winnerAnimIsActive == false && isInFishingState && !fishGame.activeSelf && !FishBited)
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
        isFishing = false;
        targetTime = 0.0f;
        timeTillCatch = 0.0f;
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
        isFishing = false;
        timeTillCatch = 0.0f;
        isInFishingState = false;
        FishBited = false;
    }


    public void fishGameWon()
    {
        FishGameResult = true;
        playerVisual.animator.Play(AnimationHashes.CAPTURE_NOFISH);
        isInFishingState = false;
        fishGame.SetActive(false);
        isFishing = false;
        FishBited = false;
        timeTillCatch = 0.0f;
    }
    public void fishGameLossed()
    {
        FishGameResult = false;
        playerVisual.animator.Play(AnimationHashes.CAPTURE_NOFISH);
        isInFishingState = false;
        fishGame.SetActive(false);
        isFishing = false;
        FishBited = false;
        timeTillCatch = 0.0f;
    }
    public void OnCastFishingEnd()
    {
        UpdateFishingPointPosition();
        temp = extraBobberDistance * new Vector3(playerMovement.HorizontalMovement, playerMovement.VerticalMovement, 0);
        fishingPoint.transform.position += temp;
        isFishing = true;
        Instantiate(bobber, fishingPoint.position, fishingPoint.rotation, transform);
        fishingPoint.transform.position -= temp;
        targetTime = 0.0f;
        extraBobberDistance = 0.0f;
    }
    private void RemovePreviousBobber()
    {
        foreach (Bobber bobber in GetComponentsInChildren<Bobber>())
        {
            Destroy(bobber.gameObject);
        }
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
