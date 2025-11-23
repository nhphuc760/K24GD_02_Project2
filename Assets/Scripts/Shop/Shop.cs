using UnityEngine;

public class Shop : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GroundCheck"))
        {
            GameEventManager.Ins.shopEvent.PlayerEnterShop();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("GroundCheck"))
        {
            GameEventManager.Ins.shopEvent.PlayerExitShop();
        }
    }
}
