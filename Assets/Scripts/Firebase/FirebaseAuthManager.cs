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
                regiterNotify.text = "Hoạt động đăng ký bị hủy";
                return;
            }else if (task.IsFaulted)
            {
                regiterNotify.text = "Đã có lỗi xảy ra, Vui lòng thử lại";
                return;
            }else if (task.IsCompleted)
            {
                regiterNotify.text = "Đăng ký thành công";

            }
        });
    }

    public void Sign()
    {
        auth.SignInWithEmailAndPasswordAsync(signEmail.text, signPassword.text).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
               
                loginNotify.text = "Hoạt động đăng nhập bị hủy";
                return;
            }
            else if (task.IsFaulted)
            {

                loginNotify.text = "Đã có lỗi xảy ra vui lòng thử lại";
                return;
            }
            else if (task.IsCompleted)
            {
                if (LoadingScene.Ins != null)
                    LoadingScene.Ins.LoadScene("MainMenu", "Loading...", LoadSceneMode.Single);
                else SceneManager.LoadScene("MainMenu");
                    signBTN.interactable = false;
            }
        });
    }
}
