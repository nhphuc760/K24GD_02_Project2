using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Scriptable Objects/CharacterDatabase")]
public class CharacterDatabase : ScriptableObject
{
    public List<CharacterDataSO> characters;
    private void OnValidate()
    {
        characters = Resources.LoadAll<CharacterDataSO>("CharacterData").ToList();
    }
}
