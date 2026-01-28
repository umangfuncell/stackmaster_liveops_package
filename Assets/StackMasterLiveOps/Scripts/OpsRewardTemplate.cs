using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OpsRewardTemplate", menuName = "SO/OpsRewardTemplate")]
public class OpsRewardTemplate : ScriptableObject
{
    public OpsRewardType type;
    public Sprite sprite;
}
