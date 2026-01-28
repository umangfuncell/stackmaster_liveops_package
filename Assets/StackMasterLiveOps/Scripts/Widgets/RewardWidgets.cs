using FunCell.GridMerge.System;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using FunCell.GridMerge.UI;
using System.Collections.Generic;
using FunCell.GridMerge.GamePlay;

public class RewardWidgets : MonoBehaviour
{
    [SerializeField] private TMP_Text _score_Label;
    [SerializeField] private TMP_Text _stage_Label;
    [SerializeField] private Image _fill_Img;
    [SerializeField] private Image _collectionImage;
    [SerializeField] private GamePanel gamePanel;

    private OpsRewardManger _manager;

    public void Open()
    {
        if (_manager == null)
        {
            _manager = OpsGameManager.Instance.RewardManger;
        }
        OnActiveRewardChange(_manager.GetActiveReward(), _manager.ActiveRewardIndex);
        OnProgressChange(_manager.StageScore, _manager.GetActiveReward().requiredScore);

        _manager.OnActiveRewardChange += OnActiveRewardChange;
        _manager.OnProgressChange += OnProgressChange;
        _manager.OnRewardGrant += OnRewardGrant;
        _collectionImage.gameObject.SetActive(false);
    }
    public void Close()
    {
        _manager.OnRewardGrant -= OnRewardGrant;
        _manager.OnActiveRewardChange -= OnActiveRewardChange;
        _manager.OnProgressChange -= OnProgressChange;
    }
    public void OnRewardGrant(OpsRewardChest chest)
    {
        gamePanel.OnRewardGrant(chest);
    }
    public void OnActiveRewardChange(OpsRewardChest reward, int activeRewardIndex)
    {
        _stage_Label.text = $"stage {activeRewardIndex + 1}";
    }
    public void OnProgressChange(int score, int target)
    {
        _score_Label.text = $"{score}/{target}";
        _fill_Img.fillAmount = (float)score / target;
    }
}
