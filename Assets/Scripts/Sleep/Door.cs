using TMPro;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public string sceneToLoad;
    public Vector3 targetPostion;
    bool isInteract;
    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        Debug.Log("Interacted with Door to " + sceneToLoad);
        if (GameManager.Ins != null)
        {
            
            GameManager.Ins.StartSceneTransition(sceneToLoad, targetPostion);
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("GroundCheck"))
    //    {
    //        isInteract = true;
    //        // Optionally, show UI prompt to interact
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("GroundCheck"))
    //    {
    //        isInteract = false;
    //        // Optionally, hide UI prompt to interact
    //    }

}
