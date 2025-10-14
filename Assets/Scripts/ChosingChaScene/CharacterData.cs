using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Character's Info")]

    public string characterName;
    


    [Header("Character's Anim")]
    public SpriteLibraryAsset SpriteLibraryAsset; // Character's sprite library asset for animations
    public Sprite portrait; // Character's portrait image for UI display
    public string idlecategory = "Idle_Front"; // Category name for idle animation
    public string idlelabel = "Idle_0"; // Label name for idle animation


    public string description; // Character's description display on scene

}
