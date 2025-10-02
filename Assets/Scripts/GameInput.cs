using UnityEngine;

public class GameInput : MonoBehaviour 
{
    public static GameInput Ins { get; private set; }
    InputSystem_Actions inputActions;
    private void Awake()
    {
       
        if(Ins == null)
        {
            Ins = this;
        }
        else
        {
            Destroy(gameObject);
        }
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
    }

    public Vector2 GetInputMovementNormalize()
    {
        return inputActions.Player.Move.ReadValue<Vector2>().normalized;
    }
    
}
