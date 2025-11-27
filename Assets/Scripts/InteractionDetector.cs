using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInrange = null; //Closest interactable
    public GameObject interactionIcon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactionIcon?.SetActive(false);
    }
    public void OnInteract()
    {
        Debug.Log("Interact With NPC");
            interactableInrange?.Interact();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInrange = interactable;
            interactionIcon?.SetActive(true);
            if(GameEventManager.Ins != null)
            GameEventManager.Ins.gameInput.interacPressed += OnInteract;
        }
    }

  

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInrange)
        {
            interactableInrange = null;
            interactionIcon?.SetActive(false);
            if(GameEventManager.Ins != null)
            GameEventManager.Ins.gameInput.interacPressed -= OnInteract;
        }
    }
}
