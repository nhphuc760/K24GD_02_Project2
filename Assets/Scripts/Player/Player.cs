using Assets.Scripts;
using Assets.Scripts.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public static Player Ins { get; private set; }
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
        if (Ins != null && Ins != this)
        {
            Destroy(this.gameObject);
            return;
        }
            Ins = this;
        DontDestroyOnLoad(this.gameObject);
        Tilemap[] allTilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        _groundTilemaps = System.Array.FindAll(allTilemaps, tm => tm.gameObject.layer == LayerMask.NameToLayer("Ground"));
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isFishing = false;
        fishGame.SetActive(false);
        targetTime = 0.0f;
        extraBobberDistance = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        isBobberInWater = CheckBobberInWater(new Vector2(PlayerMovement.Ins.HorizontalMovement, PlayerMovement.Ins.VerticalMovement));
        HandleInput();
        if (isInFishingState && !isFishing)
        {
            targetTime += Time.deltaTime;
        }
    }
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && PlayerVisual.Ins.winnerAnimIsActive == false && IsInFishingZone && !fishGame.activeSelf && !FishBited && isBobberInWater)
        {
            RemovePreviousBobber();
            StartPoleBack();
        }
        if (Input.GetKeyUp(KeyCode.Space) && PlayerVisual.Ins.winnerAnimIsActive == false && isInFishingState && !fishGame.activeSelf && !FishBited)
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
        PlayerVisual.Ins.animator.Play(AnimationHashes.POLE_BACK);
    }
    private void CastingFishing()
    {
        PlayerVisual.Ins.animator.Play(AnimationHashes.CASTING_FISHING);
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
        PlayerVisual.Ins.animator.Play(AnimationHashes.CAPTURE_NOFISH);
        isFishing = false;
        timeTillCatch = 0.0f;
        isInFishingState = false;
        FishBited = false;
    }


    public void fishGameWon()
    {
        FishGameResult = true;
        PlayerVisual.Ins.animator.Play(AnimationHashes.CAPTURE_NOFISH);
        isInFishingState = false;
        fishGame.SetActive(false);
        isFishing = false;
        FishBited = false;
        timeTillCatch = 0.0f;
    }
    public void fishGameLossed()
    {
        FishGameResult = false;
        PlayerVisual.Ins.animator.Play(AnimationHashes.CAPTURE_NOFISH);
        isInFishingState = false;
        fishGame.SetActive(false);
        isFishing = false;
        FishBited = false;
        timeTillCatch = 0.0f;
    }
    public void OnCastFishingEnd()
    {
        UpdateFishingPointPosition();
        temp = extraBobberDistance * new Vector3(PlayerMovement.Ins.HorizontalMovement, PlayerMovement.Ins.VerticalMovement, 0);
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
        var dir = PlayerMovement.Ins.facingDirection;
        switch (dir)
        {
            case FacingDirection.Front:
                fishingPoint.position = fishingPointFront.position;
                break;
            case FacingDirection.Back:
                fishingPoint.position = fishingPointBack.position;
                break;
            case FacingDirection.Left:
                fishingPoint.position = fishingPointLeft.position;
                break;
            case FacingDirection.Right:
                fishingPoint.position = fishingPointRight.position;
                break;
            default:
                Debug.Log("Undefined Direction");
                break;
        }
    }
    public IEnumerator StartMiniGameAfterAlertTime(float AlertFishBiteTime)
    {
        yield return new WaitForSeconds(AlertFishBiteTime);
        PlayerVisual.Ins.animator.Play(AnimationHashes.ROLL);
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
