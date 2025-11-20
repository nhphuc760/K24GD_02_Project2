using UnityEngine;

public class FishingZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("GroundCheck"))
            GameEventManager.Ins.FishingZoneEnter();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("GroundCheck"))
       GameEventManager.Ins.FishingZoneExit();
    }
}
