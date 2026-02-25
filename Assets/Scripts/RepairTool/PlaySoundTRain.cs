using UnityEngine;

public class PlaySoundTRain : MonoBehaviour
{
    [SerializeField] BlackSmith BlackSmith;

    public void PlaySound()
    {
        BlackSmith.PlaySoundEffect();
    }
}
