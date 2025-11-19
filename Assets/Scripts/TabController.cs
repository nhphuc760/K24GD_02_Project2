using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    [SerializeField] private Tab[] tabs;
    public GameObject[] pages;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ActivateTab(0);
    }

    public void ActivateTab(int tabNo)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].tabImage.sprite = tabs[i].normalSprite;
            pages[i].SetActive(false);
        }
        tabs[tabNo].tabImage.sprite = tabs[tabNo].pressedSprite;
        pages[tabNo].SetActive(true);
    }
}
[System.Serializable]
public struct Tab
{
    public Image tabImage;
    public Sprite normalSprite;
    public Sprite pressedSprite;
}
