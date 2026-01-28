using UnityEngine;
using System;
using FunCell.GridMerge.GamePlay;
public abstract class BaseNodeSelector : MonoBehaviour
{
    protected bool _canSelect;
    protected LayerMask _nodeMask;
    [SerializeField] protected MergeGridNode _selectedNode;

    public static event Action<MergeGridNode, PlaceMentType> OnNodeSelect;
    protected void FireNodeSelect(MergeGridNode mergeGridNode, PlaceMentType placeMentType)
    {
        OnNodeSelect?.Invoke(mergeGridNode, placeMentType);
    }
    public void CanSelect(bool value)
    {
        _canSelect = value;
    }
    public abstract void Init();
}
public enum PlaceMentType
{
    Tap,
    Drag
}