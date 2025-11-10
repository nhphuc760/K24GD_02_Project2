using System;
using UnityEngine;

public class AnimationEvent 
{
    public event Action<OreInfor> onPickAxe; // Đào quặng
    public void PickAxe(OreInfor oreInfor)
    {
        onPickAxe?.Invoke(oreInfor);
    }
    public event Action<TreeInfor> onAxe;//Chặt cây
    public void Axe(TreeInfor treeInfor)
    {
        onAxe?.Invoke(treeInfor);
    }
    public event Action treeDamage;
    public void TreeDamage()
    {
        treeDamage?.Invoke();
    }
}
