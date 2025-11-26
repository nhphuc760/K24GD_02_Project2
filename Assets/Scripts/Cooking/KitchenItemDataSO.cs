using System;
using UnityEngine;

[CreateAssetMenu(fileName = "KitchenItemDataSO", menuName = "Scriptable Objects/KitchenItemDataSO")]
public class KitchenItemDataSO : ItemDataSO
{
    public RecipeSO recipeSO;
    public string Description;
    public float buffStamina;
    public override void OnValidate()
    {
        TimeSpan spand = TimeSpan.FromSeconds(recipeSO.timeCooldown);
        _description = Description + $"\nThời gian: {spand.Hours}h{spand.Minutes}p{spand.Seconds}s\nGiá bán: {sell}$";
    }

    private void Reset()
    {
        isCanSell = true;
    }
    public override void Use(GameObject tarGet)
    {
       if(StaminaManager.instance != null)
        {
            StaminaManager.instance.RestoreStamina(buffStamina);
        }
    }
}
