using UnityEngine;
public enum NPCPortraitExpression
{
    Normal,
    Happy,
    Angry,
    Serious,
    Sad,
    Surprised
}
[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    public string npcName;
    public Sprite[] npcPortraits;
    public string[] dialogueLines;
    public bool[] autoProgressLines;
    public float autoProgressDelay = 1.5f;
    public float typingSpeed = 0.05f;
    public AudioClip voiceSound;
    public float voicePitch = 1.0f;
    public Sprite GetPortrait(NPCPortraitExpression emotion)
    {
        int index = (int)emotion;

        if (index >= 0 && index < npcPortraits.Length)
            return npcPortraits[index];

        Debug.LogWarning($"Cannot find the emotion {emotion} in NPC {npcName}");
        return null;
    }
}
