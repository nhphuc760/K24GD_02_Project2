using UnityEngine;
using UnityEngine.Events;

public class Bobber : MonoBehaviour
{
    float bobberTime;
    [SerializeField] Vector2 RangeRandomFishBiteTime;
    float randomFishBiteTime;
    public float AlertFishBitedTime = 2f;
    [SerializeField] PlayerFishing player;
    [SerializeField] PlayerVisual playerVisual;
    [SerializeField] GameObject fishIcon;
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
        fishIcon.SetActive(true);
        fishIcon.transform.position = this.transform.position + Vector3.up * .5f;
       playerVisual.animator.Play(AnimationHashes.FISH_HOOK_BLEND_TREE);
        StartCoroutine(player.StartMiniGameAfterAlertTime(AlertFishBitedTime));
    }
}
