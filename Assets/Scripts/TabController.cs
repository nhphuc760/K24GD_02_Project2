using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    public Image[] tabImages;
    public Image[] pressedTabImages;
    public GameObject[] pages;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void ActivateTab(int tabNo)
    {
        for (int i = 0; i < tabImages.Length; i++)
        {
            pages[i].SetActive(false);
        }
        pages[tabNo].SetActive(true);

    }
}
[System.Serializable]
public struct Tab
{
    public Image tabImage;
    public Image pressedTabImage;
    public GameObject page;
}
