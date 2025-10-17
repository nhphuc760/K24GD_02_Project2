using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour 
{
    public static GameInput Ins { get; private set; }
    InputSystem_Actions inputActions;
    public event Action submitPressed;
    public event Action openBagPressed;

    private void Awake()
    {
       
        if(Ins!= null && Ins != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Ins = this;
        }
            inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
        inputActions.Player.Interact.performed += _ => { Interact_performed(); };
        inputActions.Player.OpenBag.performed += _ => { OpenBag_performed(); };
    }

    private void OpenBag_performed()
    {
       openBagPressed?.Invoke();
    }

    private void Interact_performed()
    {
        submitPressed?.Invoke();
    }

   
   
    public Vector2 GetInputMovementNormalize()
    {
        return inputActions.Player.Move.ReadValue<Vector2>().normalized;
    }
    
}
