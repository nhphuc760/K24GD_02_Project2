using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpeningCutsceneController : MonoBehaviour
{
    public List<string> dialogueLines;

    public GameObject choicePanel;
    public Button choiceYesButton;
    public Button choiceNoButton;
    public TextMeshProUGUI choiceYesText;
    public TextMeshProUGUI choiceNoText;

    public string SceneName = "Farm";// ten scene load neu chon yes

    private int playerChoice = -1; // -1: no choice, 0: yes, 1: no


    private void Start()
    {
        if(DialogueManager.instance == null)
        {
            Debug.LogError("DialogueManager instance is null!");
            return;
        }

        //ẩn bảng lựa chọn ban đầu
        choicePanel.SetActive(false);

        //bắt đầu đoạn hội thoại
        StartCoroutine(PlayOpeningSequence());
    }

    IEnumerator PlayOpeningSequence()
    {
        Debug.Log("Starting opening cutscene dialogue.");
        //1. Bắt đầu đoạn hội thoại
        DialogueManager.instance.StartDialogue(dialogueLines);
        //2. Chờ đến khi đoạn hội thoại kết thúc
        yield return new WaitUntil(() => !DialogueManager.instance.IsDialogueActive());

        //3. Hiển thị bảng lựa chọn
        ShowChoices();
    }

    void ShowChoices()
    {
        Debug.Log("Hiển thị lựa chọn...");
        choicePanel.SetActive(true);

        // Gán text cho nút (ví dụ)
        choiceYesText.text = "Về quê thôi";
        choiceNoText.text = "Có lẽ chưa phải lúc...";

        // Gán sự kiện cho nút (xóa listener cũ trước để tránh lỗi)
        choiceYesButton.onClick.RemoveAllListeners();
        choiceYesButton.onClick.AddListener(() => SelectChoice(0)); // Gọi hàm SelectChoice với index 0

        choiceNoButton.onClick.RemoveAllListeners();
        choiceNoButton.onClick.AddListener(() => SelectChoice(1)); // Gọi hàm SelectChoice với index 1
    }

    void SelectChoice(int choiceIndex)
    {
        playerChoice = choiceIndex;
        choicePanel.SetActive(false); // Ẩn panel lựa chọn
        ProcessChoice(); // Xử lý lựa chọn
    }

    void ProcessChoice()
    {
        if (playerChoice == 0) // Chọn "Về Farm"
        {
            Debug.Log("Người chơi chọn Về Farm.");
            SceneManager.LoadScene(SceneName); // Chuyển sang scene Farm
        }
        else if (playerChoice == 1) // Chọn "Ở lại"
        {
            Debug.Log("Người chơi chọn Ở lại.");
            // Kết thúc game
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }
    }
}
