using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Scriptable Objects/CharacterDatabase")]
public class CharacterDatabase : ScriptableObject
{
    public List<CharacterData> characters;
}
