using System;
using UnityEngine;
public class QuestStepState 
{
    public string questStateDynamic;
    public QuestStepState(string current)
    {
        questStateDynamic = current;
    }
    public QuestStepState()
    {
        questStateDynamic = null;
    }
}
