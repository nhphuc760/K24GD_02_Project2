using UnityEngine;
using UnityEngine.Events;

public class Bobber : MonoBehaviour
{
    public Animator bobberAnim;
    float bobberTime;
    [SerializeField] Vector2 RangeRandomFishBiteTime;
    float randomFishBiteTime;
    public float AlertFishBitedTime = 2f;
    [SerializeField] Player player;
    [SerializeField] PlayerVisual playerVisual;

    private void OnEnable()
    {
        randomFishBiteTime = UnityEngine.Random.Range(RangeRandomFishBiteTime.x, RangeRandomFishBiteTime.y);
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
                OnFishBiteHandler();
            }
        }
    }
    private void OnFishBiteHandler()
    {
        player.FishBited = true;
        bobberAnim.Play(AnimationHashes.BOBBER_FISH);
       playerVisual.animator.Play(AnimationHashes.FISH_HOOK_BLEND_TREE);
        StartCoroutine(player.StartMiniGameAfterAlertTime(AlertFishBitedTime));
    }
}
