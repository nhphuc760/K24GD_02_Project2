using TMPro;
using UnityEngine;

public class Dialog : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogText;
    void Start()
    {
        GameEventManager.Ins.triggerDialog += Ins_triggerDialog;
        gameObject.SetActive(false);
    }

    private void Ins_triggerDialog(string log)
    {
  
        dialogText.text = log;
            gameObject.SetActive(true);
        
    }

    public void SetActiveFalse()
    {
    
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        GameEventManager.Ins.triggerDialog -= Ins_triggerDialog;
    }
}
