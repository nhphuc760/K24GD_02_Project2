using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class CharacterSelectManagement : MonoBehaviour
{
    public CharacterDatabase database;

    [Header("UI")]
    public Image portraitChar;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_InputField playerNameInput;

    [Header("Character Info")]
    public GameObject characterPreview; // GameObject to display character's sprite
    private int currentIndex = 0;

    [SerializeField] string nextScene;


    private void Start()
    {
        if(database == null || database.characters.Count == 0)
        {
            Debug.LogError("Character database is empty or not assigned.");
            return;
        }
        UpdateUI();
    }
    public void NextCharacter()
    {
        currentIndex = (currentIndex + 1) % database.characters.Count;
        UpdateUI();
    }
    public void PreviousCharacter()
    {
        currentIndex = (currentIndex - 1 + database.characters.Count) % database.characters.Count;
        UpdateUI();
    }

    /// <summary>
    /// Phụ thuộc vào dữ liệu CharacterData
    /// </summary>
    private void UpdateUI()
    {
        CharacterDataSO data = database.characters[currentIndex];
        nameText.text = data.characterName;
        descriptionText.text = data.description;
        portraitChar.sprite = data.portrait;
        var lib = characterPreview.GetComponent<SpriteLibrary>();
        var resolver = characterPreview.GetComponent<SpriteResolver>();
        // Update character preview sprite and animation
        if (lib != null)
        {
            lib.spriteLibraryAsset = data.SpriteLibraryAsset;
            resolver.SetCategoryAndLabel(data.idlecategory, data.idlelabel);
        }
        else
        {
            Debug.LogError("SpriteLibrary or SpriteResolver component is missing on the characterPreview GameObject.");
        }

        //add anim
        var animator = characterPreview.GetComponent<SimpleSpriteAnimation>();
        if (animator != null)
        {
            animator.category = data.idlecategory;
        }
    }

    public async void ConfirmSelection()
    {
        Debug.Log($"[KIỂM TRA] Giá trị của playerName ngay khi nhấn Confirm là: '{playerNameInput.text}'");
        await Save_Load_Firebase.RemoveAllDataBase();
        if (string.IsNullOrWhiteSpace(playerNameInput.text))
        {
            Debug.LogWarning("Player name is empty. Please enter a name.");
            return;
        }
        PlayerData player = new PlayerData
        {
            _name = playerNameInput.text,
            indexAsset = currentIndex
        };
        await Save_Load_Firebase.SaveData("PlayerData", player.ToString());
        if (LoadingScene.Ins != null)
            LoadingScene.Ins.LoadScene(nextScene, "Loading...", LoadSceneMode.Single, false);
        else SceneManager.LoadScene(nextScene);
    }
}
