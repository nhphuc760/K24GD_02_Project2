using UnityEngine;

public class NpcBuy_Sell : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameEventManager.Ins.OnNearSell(true);
            GameEventManager.Ins.inventoryEvent.OpenBagPress();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameEventManager.Ins.OnNearSell(false);
            GameEventManager.Ins.inventoryEvent.DisableInventory();
        }
    }

}
