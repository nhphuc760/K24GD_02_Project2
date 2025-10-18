
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Editor Test Data")]
    public string testPlayerName = "Test Dũng";
    public Sprite testPlayerAvatar;
    public SpriteLibraryAsset testSpriteLibrary;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //Để Script giúp cho GManager "lắng nghe" sự kiện khi scene thay đổi không bị mất đi
    // Đăng ký "lắng nghe" sự kiện khi scene thay đổi
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Hủy đăng ký để tránh lỗi
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Hàm này sẽ được tự động gọi MỖI KHI một scene mới được tải xong.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Chỉ chạy logic này khi chúng ta vào một scene game (không phải Main Menu hay CharacterSelect),login,loadscene...
        if (scene.name != "PersistentSystems" && scene.name != "CustomizeCharacter")
        {
            // Kiểm tra xem có phải đang chạy test và PlayerDataHolder có trống không
            if (Application.isEditor && string.IsNullOrWhiteSpace(PlayerDataHolder.instance.selectedCharacterName))
            {
                Debug.LogWarning("--- GameManager: Tải dữ liệu TEST ---");
                LoadTestData();
            }
            else
            {
                Debug.Log("--- GameManager: Tải dữ liệu GAME MỚI ---");
                LoadNewGameData();
            }
        }
    }
    // Hàm để tải dữ liệu test trong Editor phòng khi không chạy từ hàm select character mà muốn test thẳng
    private void LoadTestData()
    {
        if (UIManager.instance != null)
            UIManager.instance.SetupPlayerInfo(testPlayerName, testPlayerAvatar);

        PlayerCharacterChanger changer = FindObjectOfType<PlayerCharacterChanger>();
        if (changer != null && testSpriteLibrary != null)
            changer.ApplyCharacterData(testSpriteLibrary);
    }
    //h̀n để tải dữ liệu khi bắt đầu game mới
    private void LoadNewGameData()
    {
        if (PlayerDataHolder.instance == null) return;

        string nameFromSelection = PlayerDataHolder.instance.selectedCharacterName;
        Sprite avatarFromSelection = PlayerDataHolder.instance.selectedPlayerAvatar;

        if (UIManager.instance != null)
            UIManager.instance.SetupPlayerInfo(nameFromSelection, avatarFromSelection);
    }
}