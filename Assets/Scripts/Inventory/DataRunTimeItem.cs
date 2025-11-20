using System;
using UnityEngine;

public abstract class DataRunTimeItem
{
   
    public abstract string SerializeData();
    public abstract override string ToString();
    public abstract void Init(ItemDataSO itemDataSO);
}

