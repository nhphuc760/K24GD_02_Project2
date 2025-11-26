using System;
using UnityEngine;

[Serializable]
public class CollectionData
{
    public int current;
    public int target;
    public override string ToString()
    {
        return $"{current}/{target}";
    }
}
