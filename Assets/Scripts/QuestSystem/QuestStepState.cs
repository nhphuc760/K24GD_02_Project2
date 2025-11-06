using System;
using UnityEngine;
public class QuestStepState 
{
    public int current;
    public int target;
    public QuestStepState(int current)
    {
        this.current = current;
    }
    public QuestStepState()
    {
        this.current = 0;
    }
    public override string ToString()
    {
        return $"{current}/{target}";
    }
}
