using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Scriptable Objects/CharacterDatabase")]
public class CharacterDatabase : ScriptableObject
{
    public List<CharacterData> characters;
    private void OnValidate()
    {
        characters = Resources.LoadAll<CharacterData>("CharacterData").ToList();
    }
}
