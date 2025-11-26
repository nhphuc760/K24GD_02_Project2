using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("Setting")]
    public float typingSpeed = 0.04f;

    private Queue<string> sentences; // hang doi cac cau thoai
    private bool isTyping = false; // kiem tra dang go chu chua
    private string currentSentence; // luu cau thoai dang duoc go chu
    private Coroutine typingCoroutine; // bien luu tru coroutine dang chay

    //bien bao hieu doi thoai da ket thuc (Cho cutsceneController biet)
    private bool dialogueFinished = true;

    public AudioClip typingSound; // Kéo file âm thanh tiếng gõ vào đây
    public int soundFrequency = 2;
    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        sentences = new Queue<string>();
    }

    private void Update()
    {
        //xu ly click chuot de hien thi cau tiep theo hoac bo qua viec go chu
        if (dialoguePanel.activeSelf && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            if (isTyping) // neu dang go --> hien het chu
            {
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                dialogueText.text = currentSentence;
                isTyping = false;
            }
            else // neu khong dang go --> hien cau tiep theo
            {
                DisplayNextSentence();
            }
        }
    }
    //bat dau doi thoai voi danh sach cau thoai
    public void StartDialogue(List<string> dialogueLines)
    {
        dialogueFinished = false;
        sentences.Clear();

        foreach(string sentence in dialogueLines)
        {
            sentences.Enqueue(sentence);//them tung cau thoai vao hang doi
        }

        dialoguePanel.SetActive(true);
        DisplayNextSentence();
    }

    //hien thi cau thoai tiep theo
    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)//neu hang doi rong
        {
            EndDialogue();
            return;
        }

        currentSentence = sentences.Dequeue();//lay cau thoai tiep theo trong hang doi
        if(typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);//dung coroutine neu dang chay
        }
        typingCoroutine = StartCoroutine(TypeSentence(currentSentence));//bat dau go chu
    }

    //go chu tung ky tu mot
    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        int charIndex = 0;
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            if (typingSound != null && letter != ' ' && charIndex % soundFrequency == 0)
            {
                PlayTypingSound();
            }
            charIndex++;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }
    void PlayTypingSound()
    {
        if (AudioManager.instance != null)
        {
            // Nếu bạn đã làm hàm PlaySFXRandomPitch như bài trước thì dùng cái đó sẽ hay hơn
            // AudioManager.instance.PlaySFXRandomPitch(typingSound);

            // Nếu chưa thì dùng PlaySFX thường
            AudioManager.instance.PlayFX(typingSound);
        }
    }
    //ket thuc doi thoai
    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        dialogueFinished = true;
        Debug.Log("Ket thuc doi thoai");
    }

    //kiem tra xem doi thoai co dang hoat dong khong
    public bool IsDialogueActive()
    {
        return !dialogueFinished;
    }


}
