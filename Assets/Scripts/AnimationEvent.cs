using System;
using UnityEngine;

public class AnimationEvent 
{
    public event Action<OreInfor> onPickAxe;
    public void PickAxe(OreInfor oreInfor)
    {
        onPickAxe?.Invoke(oreInfor);
    }
    public event Action onAxe;//Chặt cây
    public void Axe()
    {
        onAxe?.Invoke();
    }
    public event Action treeDamage;
    public void TreeDamage()
    {
        treeDamage?.Invoke();
    }
}
