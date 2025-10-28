using UnityEngine;

[CreateAssetMenu(fileName = "KitchenItemDataSO", menuName = "Scriptable Objects/KitchenItemDataSO")]
public class KitchenItemDataSO : ItemDataSO
{
    public RecipeSO recipeSO;
    public int price;
}
