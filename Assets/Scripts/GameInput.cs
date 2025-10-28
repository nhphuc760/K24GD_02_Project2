using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameInput : MonoBehaviour 
{
    public static GameInput Ins { get; private set; }
    public InputActionAsset inputActionAsset;
    private InputAction moveAction;
    public event Action submitPressed;
    public event Action openBagPressed;
    public event Action openInventoryPressed;


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
            Debug.Log("GameInput Awake and set as singleton");
        }
        if (inputActionAsset == null)
        {
            inputActionAsset = Resources.Load<InputActionAsset>("InputSystem_Actions");                                             
        }
        if (inputActionAsset != null)
        {
            moveAction = inputActionAsset.FindAction("Move");
            if (moveAction != null) moveAction.Enable();
            var interactAction = inputActionAsset.FindAction("Interact");                                                               
            if (interactAction != null) interactAction.performed += _ => { Interact_performed(); };                                     
            var openBagAction = inputActionAsset.FindAction("OpenBag");                                                                 
            if (openBagAction != null) openBagAction.performed += _ => { OpenBag_performed(); };                                    
        }
    }

    private void OpenBag_performed()
    {
       openBagPressed?.Invoke();
    }

    private void Interact_performed()
    {
        Debug.Log("OnSubmit pressed");
        submitPressed?.Invoke();
    }

    public Vector2 GetInputMovementNormalize()
    {
        if (moveAction != null)
        {
            return moveAction.ReadValue<Vector2>().normalized;
        }
        return Vector2.zero;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F pressed in GameInput, loading Store");
            SceneManager.LoadScene("Store");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed in GameInput");
            openInventoryPressed?.Invoke();
        }
    }
    
}