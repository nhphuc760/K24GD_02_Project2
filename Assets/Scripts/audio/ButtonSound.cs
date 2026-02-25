using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [Header("Audio Clips")]
    public AudioClip clickSound;
    public AudioClip hoverSound;

    [Header("Settings")]
    [Range(0f, 1f)] public float volumeScale = 1f;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;
        PlaySound(clickSound);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;
        PlaySound(hoverSound);
    }

 
    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;

        //Có AudioManager
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayFX(clip);
        }
        //Không có AudioManager
        else
        {
            float savedVolume = PlayerPrefs.GetFloat("SFXVol", 1f);
            AudioSource.PlayClipAtPoint(clip, Vector3.zero, volumeScale * savedVolume);
        }
    }
}