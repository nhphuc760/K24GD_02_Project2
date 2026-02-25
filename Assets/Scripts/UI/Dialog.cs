using TMPro;
using UnityEngine;

public class Dialog : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] RectTransform backGround;
    [SerializeField] float textPadding = 80;
    Vector2 pivotDefault = new Vector2(0, 1);// top_left
    void Start()
    {
        GameEventManager.Ins.triggerDialog += Ins_triggerDialog;
        gameObject.SetActive(false);
    }

    private void Ins_triggerDialog(string log)
    {
  
            dialogText.text = log;
          gameObject.SetActive(true);
         FixedContentSize();
    }


    void FixedContentSize()
    {
        backGround.sizeDelta = new Vector2(dialogText.preferredWidth + textPadding *2f, dialogText.preferredHeight + textPadding *2f);
        dialogText.rectTransform.pivot = pivotDefault;  

    }
    public void SetActiveFalse()
    {
    
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        if(GameEventManager.Ins != null)
        GameEventManager.Ins.triggerDialog -= Ins_triggerDialog;
    }
}
