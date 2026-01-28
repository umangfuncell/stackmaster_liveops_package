using System;
using System.Collections.Generic;
using FunCell.GridMerge.System;
using UnityEngine;

public class OpsRewardManger : MonoBehaviour
{
    [SerializeField] private List<OpsRewardChest> _rewards;
    private int _activeRewardIndex = 0;
    public int ActiveRewardIndex => _activeRewardIndex;

    public event Action<OpsRewardChest> OnRewardGrant;
    public event Action<OpsRewardChest, int> OnActiveRewardChange;
    public event Action<int, int> OnProgressChange;

    public int StageScore { get; private set; }

    public void InIt()
    {
        OpsDataManager.OnScoreChange += OnScoreChange;
        SetStageScore(OpsDataManager.Score);
    }
    public void OnDestroy()
    {
        OpsDataManager.OnScoreChange -= OnScoreChange;
    }
    public void OnScoreChange(int score)
    {
        if (_activeRewardIndex < _rewards.Count)
        {
            OpsRewardChest reward = _rewards[_activeRewardIndex];
            SetStageScore(score);
            OnProgressChange?.Invoke(StageScore, reward.requiredScore);
            if (StageScore >= reward.requiredScore)
            {
                OnRewardGrant?.Invoke(reward);
                _activeRewardIndex++;
                SetStageScore(score);
                OnActiveRewardChange?.Invoke(_rewards[_activeRewardIndex], _activeRewardIndex);
                OnProgressChange?.Invoke(StageScore, reward.requiredScore);
            }
        }
    }
    public OpsRewardChest GetActiveReward()
    {
        return _activeRewardIndex < _rewards.Count ? _rewards[_activeRewardIndex] : null;
    }
    private void SetStageScore(int score)
    {
        StageScore = score;
        for (int i = 0; i < _activeRewardIndex; i++)
        {
            StageScore -= _rewards[i].requiredScore;
        }
        // if (_activeRewardIndex - 1 >= 0)
        // {
        //     StageScore = score - _rewards[_activeRewardIndex - 1].requiredScore;
        // }
        // else
        // {
        //     StageScore = score;
        // }
    }

}
[Serializable]
public class OpsRewardChest
{
    public int requiredScore;
    public List<OpsReward> rewards;
}
[Serializable]
public class OpsReward
{
    public OpsRewardTemplate template;
    public int rewardAmount;
}
public enum OpsRewardType
{
    Diamond,
    Currency,
    SpinWheel,
    AdTicket,
    BizCash,
    MoonStone,
    Coins,
    Tokens,
    Cards,
}