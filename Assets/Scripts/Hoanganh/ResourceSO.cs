using UnityEngine;

[CreateAssetMenu(fileName = "NewResource", menuName = "Scriptable Objects/ResourceSO")]
public class ResourceSO : ItemDataSO
{
    public GameObject prefabObj;
    public override void OnValidate()
    {
        
    }
}
