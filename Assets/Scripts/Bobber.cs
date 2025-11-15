using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.Events;

public class Bobber : MonoBehaviour
{
    public static event UnityAction OnFishBiteAlert;
    public bool GameIsOver;
    public Animator bobberAnim;
    public float bobberTime;
    public float randomFishBiteTime = 0;
    public float AlertFishBitedTime = 2f;
    private void Awake()
    {
        OnFishBiteAlert += OnFishBiteHandler;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        randomFishBiteTime = UnityEngine.Random.Range(5f, 6f);
        bobberTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (bobberTime < randomFishBiteTime)
        {
            bobberTime += Time.deltaTime;
            if (bobberTime >= randomFishBiteTime)
            {
                OnFishBiteAlert?.Invoke();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.P) && Player.Ins.isInFishingState)
        {
            Destroy(this.gameObject);
        }
        if (GameIsOver == true)
        {
            Destroy(this.gameObject);
        }
    }
    private void OnDestroy()
    {
        OnFishBiteAlert -= OnFishBiteHandler;
    }
    private void OnFishBiteHandler()
    {
        Player.Ins.FishBited = true;
        bobberAnim.Play(AnimationHashes.BOBBER_FISH);
        PlayerVisual.Ins.animator.Play(AnimationHashes.FISH_HOOK_BLEND_TREE);
        StartCoroutine(Player.Ins.StartMiniGameAfterAlertTime(AlertFishBitedTime));
    }
    public void gameOver()
    {
        GameIsOver = true;
    }
}
