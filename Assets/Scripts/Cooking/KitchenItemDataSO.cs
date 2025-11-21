using UnityEngine;

[CreateAssetMenu(fileName = "KitchenItemDataSO", menuName = "Scriptable Objects/KitchenItemDataSO")]
public class KitchenItemDataSO : ItemDataSO
{
    public RecipeSO recipeSO;
    public int sale_Price;
    public string Description;
    public override void OnValidate()
    {
        _description = Description + $"\nThời gian: {recipeSO.timeCooldown}\nGiá bán: {sale_Price}";
    }
}
