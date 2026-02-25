using UnityEngine;

public class NpcBuy_Sell : MonoBehaviour, IInteractable
{

   
    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        GameEventManager.Ins.inventoryEvent.OpenBagPress();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameEventManager.Ins.OnNearSell(true);
           
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
