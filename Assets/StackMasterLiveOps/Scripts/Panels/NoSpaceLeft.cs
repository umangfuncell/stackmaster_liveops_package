using UnityEngine;
using UnityEngine.UI;
using FunCell.GridMerge.GamePlay;
using TMPro;
using FunCell.GridMerge.System;

namespace FunCell.GridMerge.UI
{
    public class NoSpaceLeft : Panel
    {
        public override void Open()
        {
            base.Open();
            LeanTween.delayedCall(2f, () =>
            {
                OpsSoundManager.Instance.PlayAchievementSfx();
                PanelManager.Instance.Open<LeaderboardPanel>();
                //umang
                //LevelBasedProgressionRelated.LogLevelEndEventWithTime();
                //OpsAnalyticsManager.Instance.LogEvent("liveops:leaderboard_display");
                //PanelManager.Instance.Open<LeaderBoardV2Panel>();
                // if (OpsDataManager.Score >= OpsDataManager.HighScore)
                // {
                // }
                // else
                // {
                //     OpsSoundManager.Instance.PlayGameover();
                //     PanelManager.Instance.Open<LiveOpsPopup>();
                // }
            });
        }
        public override void Close()
        {
            base.Close();
        }
    }
}
