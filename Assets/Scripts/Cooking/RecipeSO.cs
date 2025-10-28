using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;


[CreateAssetMenu(fileName = "RecipeSO", menuName = "Scriptable Objects/RecipeSO")]
public class RecipeSO : ScriptableObject
{
    [Serializable]
    public class Ingredient
    {
        public ItemDataSO itemSO;
        public int quantity;
    }
    public float timeCooldown;
    public List<Ingredient> ingredients;
    public ItemDataSO result;
}
