using UnityEngine;


[CreateAssetMenu(fileName = "NewFishSO", menuName = "Scriptable Objects/FishDataSO")]
public class FishDataSO : ItemDataSO
{
    public string Description;
    private void Reset()
    {
        isCanSell = true;
    }
    public override void OnValidate()
    {
        _description = Description + $"\nGiá bán: {sell}$/1";
    }
}
