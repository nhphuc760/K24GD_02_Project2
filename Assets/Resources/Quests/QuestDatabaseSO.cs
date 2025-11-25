using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestDatabase", menuName = "Scriptable Objects/Quest/QuestDatabaseSO")]
public class QuestDatabaseSO : ScriptableObject
{
    public List<QuestInforSO> questDatabase;
    public QuestInforSO GetQuestByID(string id)
    {
        return questDatabase.Find(x=> x._id == id);
    }
    private void OnValidate()
    {
        questDatabase = Resources.LoadAll<QuestInforSO>("Quests").ToList();
    }
}
