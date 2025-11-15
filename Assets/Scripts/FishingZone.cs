using UnityEngine;

public class FishingZone : MonoBehaviour
{
    public static FishingZone Ins { get; private set; }
    public bool playerIsInFishingZone = false;

    private void Awake()
    {
        if (Ins != null && Ins != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Ins = this;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerIsInFishingZone = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        playerIsInFishingZone = false;
    }
}
