using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestInfor", menuName = "ScriptableObjects/QuestInfor")]
public class QuestInfoSO : ScriptableObject
{
    [field: SerializeField]
    public string id { get; private set; }

    [Header("Generall")]
    public string DisplayName;//Description
    [Header("Requirements")]
    public int levelRequirement;
    public QuestInfoSO[] questPrerequisites;
    [Header("Steps")]
    public GameObject[] questStepPrefabs;
    [Header("Rewards")]
    public int gold;
    public int exp;


    

    private void OnValidate()
    {
#if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
