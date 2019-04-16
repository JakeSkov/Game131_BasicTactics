using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DerivedStatList : ScriptableObject
{
    public DerivedStat[] stats;
    public int Length
    {
        get
        {
            return stats == null ? 0 : stats.Length;
        }
    }
}

[System.Serializable]
public class DerivedStat
{
    public string name = string.Empty;
    public string expression = string.Empty;


}


