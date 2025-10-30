using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameInput : MonoBehaviour 
{
    public static GameInput Ins { get; private set; }
    public InputActionAsset inputActionAsset;
    private InputAction moveAction;
    public event Action interacPressed;


    private void Awake()
    {
       
        if(Ins!= null && Ins != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Ins = this;
            DontDestroyOnLoad(this.gameObject);
        }
      
        if (inputActionAsset != null)
        {
            moveAction = inputActionAsset.FindAction("Move");
            if (moveAction != null) moveAction.Enable();
            var interactAction = inputActionAsset.FindAction("Interact");                                                               
            if (interactAction != null) 
                interactAction.performed += _ => { Interact_performed(); };                                     
            var openBagAction = inputActionAsset.FindAction("OpenBag");                                                                 
            if (openBagAction != null) 
                openBagAction.performed += _ => { GameEventManager.Ins.inventoryEvent.OpenBagPress(); };                                    
        }
    }

    private void Interact_performed()
    {
        Debug.Log("OnSubmit pressed");
        interacPressed?.Invoke();
    }

    public Vector2 GetInputMovementNormalize()
    {
        if (moveAction != null)
        {
            return moveAction.ReadValue<Vector2>().normalized;
        }
        return Vector2.zero;
    }
    
}