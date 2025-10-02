using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirebaseAuthManager : MonoBehaviour
{
    [Header("Register")]
   
    [SerializeField] InputField registerEmail;
    [SerializeField] InputField registerPassword;
    [SerializeField] Text regiterNotify;
    [SerializeField] Button registerBTN;
    [Header("Sign")]
   
    [SerializeField] InputField signEmail;
    [SerializeField] InputField signPassword;
    [SerializeField] Text loginNotify;
    [SerializeField] Button signBTN;
    FirebaseAuth auth;
    private void Awake()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;
    }
    private void Start()
    {
        registerBTN.onClick.AddListener(Register);
        signBTN.onClick.AddListener(Sign);
        
    }
    public void Register()
    {
        auth.CreateUserWithEmailAndPasswordAsync(registerEmail.text, registerPassword.text).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                regiterNotify.text = "The registration has been canceled";
                return;
            }else if (task.IsFaulted)
            {
                regiterNotify.text = "An error has occurred, please try again";
                return;
            }else if (task.IsCompleted)
            {
                regiterNotify.text = "Registration successful";
            }
        });
    }

    public void Sign()
    {
        auth.SignInWithEmailAndPasswordAsync(signEmail.text, signPassword.text).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
               
                loginNotify.text = "The sign in has been canceled";
                return;
            }
            else if (task.IsFaulted)
            {

                loginNotify.text = "An error has occurred, please try again";
                return;
            }
            else if (task.IsCompleted)
            {
                SceneManager.LoadScene("LoadingScene");
            }
        });
    }
}
