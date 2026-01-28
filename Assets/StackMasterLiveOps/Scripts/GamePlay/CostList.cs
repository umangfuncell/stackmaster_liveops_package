using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CostList", menuName = "SO/CostList", order = 3)]
public class CostList : ScriptableObject
{
    public List<int> list;
}
