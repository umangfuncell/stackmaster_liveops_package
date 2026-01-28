using System.Collections;
using System.Collections.Generic;
using FunCell.GridMerge.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardFrame : MonoBehaviour
{
    [SerializeField] private TMP_Text _rewardAmount_Label;
    [SerializeField] private Image _reward_Img;
    public void Show()
    {
        GetComponent<Image>().enabled = true;
        _reward_Img.enabled = true;
        _rewardAmount_Label.enabled = true;
    }
    public void Hide()
    {
        GetComponent<Image>().enabled = false;
        _reward_Img.enabled = false;
        _rewardAmount_Label.enabled = false;
    }
    public void SetData(OpsReward reward)
    {
        _rewardAmount_Label.text = reward.rewardAmount.ToString();
        _reward_Img.sprite = reward.template.sprite;
    }
}
