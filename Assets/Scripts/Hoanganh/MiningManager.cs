using System;
using UnityEngine;

public class MiningManager : MonoBehaviour
{
    public static MiningManager Ins;
    public AudioClip[] breakSound;
    private void Awake()
    {
       Ins = this;
    }

}
