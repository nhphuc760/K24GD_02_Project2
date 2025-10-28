using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ListRecipeSO", menuName = "Scriptable Objects/RecipeListSO")]
public class RecipeListSO : ScriptableObject
{
    public List<RecipeSO> recipeSOlist;
    public RecipeSO GetRecipeSOByIdResult(int id)
    {
        return recipeSOlist.Find(x => x.result._id.Equals(id));
    }
    private void OnValidate()
    {
        recipeSOlist = Resources.LoadAll<RecipeSO>("Cooking/Recipe").ToList();
    }
}
