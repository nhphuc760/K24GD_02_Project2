using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] RectTransform background;
    [SerializeField] Canvas canvas;
    [SerializeField] RectTransform rectThis;
    RectTransform canvasRect;
    Vector2 pivotDefault = new Vector2(0, 1);// top_left
    private void Awake()
    {
        if(rectThis == null)
        {
            rectThis = GetComponent<RectTransform>();
        }
        canvasRect = canvas.GetComponent<RectTransform>();
        
    }

    private void Start()
    {
        GameEventManager.Ins.shopEvent.onShowToolTip += ShowToolTip;
        GameEventManager.Ins.shopEvent.onPointerExit += HideToolTip;
        HideToolTip();
    }

    void ShowToolTip(string message)
    {
        gameObject.SetActive(true);
        //set default anchor
        rectThis.pivot = pivotDefault;
        //updateUI
        description.text = message;
        float textPaddingSize = 4f;
        background.sizeDelta = new Vector2(description.preferredWidth + textPaddingSize *2f, description.preferredHeight + textPaddingSize *2f);
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, canvas.worldCamera, out localPos);
        //CheckVisible
        //Kiểm tra biên phải
        Vector2 pivotSetup = Vector2.zero;
        if(localPos.x + background.rect.width > canvasRect.rect.width/2f)
        {
            pivotSetup.x = 1;
        }
        if (localPos.y - background.rect.height < canvasRect.rect.height/2f)
        {
            pivotSetup.y = 0;
        }
        rectThis.pivot = pivotSetup;
        rectThis.anchoredPosition = localPos;
    }
    void HideToolTip()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {

        GameEventManager.Ins.shopEvent.onShowToolTip -= ShowToolTip;
        GameEventManager.Ins.shopEvent.onPointerExit -= HideToolTip;
    }
}
