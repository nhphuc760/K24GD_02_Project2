using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public Image portraitImage;
    public TMP_Text nameText, dialogueText;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive = false;
    private string currentLine;
    private NPCPortraitExpression currentExpression;

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if (dialogueData == null || !isDialogueActive)
            return;
        if (isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }
    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        nameText.SetText(dialogueData.npcName);
        portraitImage.sprite = dialogueData.GetPortrait(NPCPortraitExpression.Normal);
        dialoguePanel.SetActive(true);

        StartCoroutine(TypeLine());
    }
    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.SetText(currentLine);
            portraitImage.sprite = dialogueData.GetPortrait(currentExpression);
            isTyping = false;
        }
        else if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }
    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");

        string rawLine = dialogueData.dialogueLines[dialogueIndex];

        NPCPortraitExpression expression = NPCPortraitExpression.Normal;
        int startIndex = rawLine.IndexOf('[');
        int endIndex = rawLine.IndexOf(']');

        if (startIndex != -1 && endIndex != -1)
        {
            string tag = rawLine.Substring(startIndex + 1, endIndex - startIndex - 1);
            if (System.Enum.TryParse(tag, true, out NPCPortraitExpression parsedEmotion))
                expression = parsedEmotion;

            rawLine = rawLine.Remove(startIndex, endIndex - startIndex + 1);
        }
        currentLine = rawLine;
        currentExpression = expression;
        portraitImage.sprite = dialogueData.GetPortrait(expression);

        foreach (char letter in rawLine)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        if (dialogueData.dialogueLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }
    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
    }
}
