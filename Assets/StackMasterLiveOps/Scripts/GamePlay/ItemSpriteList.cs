using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSpriteList", menuName = "SO/ItemSpriteList", order = 2)]
public class ItemSpriteList : ScriptableObject
{
    public List<Sprite> sprites;
}
