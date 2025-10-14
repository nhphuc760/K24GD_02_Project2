using TMPro;
using UnityEngine;
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
    private SpriteLibrary spriteLibrary;
    private SpriteResolver spriteResolver;
    public GameObject characterPreview; // GameObject to display character's sprite
    private int currentIndex = 0;
    private string playerName = "";




    private void Start()
    {
        if(database == null || database.characters.Count == 0)
        {
            Debug.LogError("Character database is empty or not assigned.");
            return;
        }
        //var lib = characterPreview.GetComponent<SpriteLibrary>();
        //var resolver = characterPreview.GetComponent<SpriteResolver>();
        UpdateUI();
    }
    public void NextCharacter()
    {
        currentIndex = (currentIndex + 1) % database.characters.Count;
        UpdateUI();
    }

    public void OnNameChanged(string newName)
    {
        playerName = newName;
    }
    public void PreviousCharacter()
    {
        currentIndex = (currentIndex - 1 + database.characters.Count) % database.characters.Count;
        UpdateUI();
    }
    private void UpdateUI()
    {
        CharacterData data = database.characters[currentIndex];
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

    public void ConfirmSelection()
    {
        if (string.IsNullOrWhiteSpace(playerName))
        {
            Debug.LogWarning("Player name is empty. Please enter a name.");
            return;
        }
        CharacterData selectedCharacter = database.characters[currentIndex];
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.SetString("SelectedCharacter", selectedCharacter.characterName);
        Debug.Log($"Character Selected: {selectedCharacter.characterName} with Player Name: {playerName}");
        // Here you can add code to save the selected character and player name to a persistent game manager or pass it to the next scene.
    }
}
