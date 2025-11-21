using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput
{
    InputSystem_Actions inputAction;
    public event Action interacPressed;
    
    public GameInput()
    {
        inputAction = new InputSystem_Actions();
    }

    public GameInput(InputSystem_Actions inputActions)
    {
        this.inputAction = inputActions;
       
    }

    public void Init()
    {
        inputAction.Player.Enable();
        inputAction.Player.OpenBag.performed += _ => { GameEventManager.Ins.inventoryEvent.OpenBagPress(); };
        inputAction.Player.Interact.performed += _ => { Interact_performed(); };
        inputAction.Player.SelectToolKit.performed += SelectToolKit_performed;
        inputAction.Player.DisplayToolKit.performed += _ => { GameEventManager.Ins.toolKitEvent.Trigger(); };
    }

    private void SelectToolKit_performed(InputAction.CallbackContext context)
    {
        string key = context.control.name;
        if(int.TryParse(key, out int num))
        {
            GameEventManager.Ins.toolKitEvent.CallInput(num);
        }
    }

    private void Interact_performed()
    {
        if(interacPressed == null)
        {
            Debug.Log("interact is not asign");
        }
       
        interacPressed?.Invoke();
    }

    public Vector2 GetInputMovementNormalize()
    {
        return inputAction.Player.Move.ReadValue<Vector2>().normalized;
    }
    public void Enable_InputAction()
    {
        inputAction.Player.Enable();
    }

    public void Disable_InputAction()
    {
        inputAction.Player.Disable();
    }
    public void DisableMovement()
    {
        inputAction.Player.Move.Disable();
    }

    public void EnableMovement()
    {
        inputAction.Player.Move.Enable();
    }
}