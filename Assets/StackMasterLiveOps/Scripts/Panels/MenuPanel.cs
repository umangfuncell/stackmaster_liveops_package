using UnityEngine;
using UnityEngine.UI;
using FunCell.GridMerge.GamePlay;
using FunCell.GridMerge.System;
using TMPro;
namespace FunCell.GridMerge.UI
{

    public class MenuPanel : Panel
    {
        [SerializeField] private Button _start_Btn;
        [SerializeField] private Button _loadGame_Btn;
        [SerializeField] public TMP_Text _DaysTxt;
        public override void Open()
        {
            base.Open();

            _start_Btn.onClick.AddListener(OnStartClick);
            _loadGame_Btn.onClick.AddListener(OnLoadGameClick);

            // _loadGame_Btn.interactable=OpsDataManager.CanContinueGame();

            //umang
            // if (OpsTimeManager.Instance.NoOfDays <= 1.0)
            // {
            //     _DaysTxt.text = "1 day left";
            // }
            // else
            // {
            //     _DaysTxt.text = (((int)OpsTimeManager.Instance.NoOfDays)+1).ToString() + " days left";
            // }
        }
        public override void Close()
        {
            base.Close();

            _start_Btn.onClick.RemoveListener(OnStartClick);
            _loadGame_Btn.onClick.RemoveListener(OnLoadGameClick);
        }
        private void OnStartClick()
        {
            OpsAnalyticsManager.Instance.LogEvent("liveops:start_clicked");
            //OpsGameManager.Instance.NewGame();
            OpsSoundManager.Instance.PlayBtnClickSound();
            PanelManager.Instance.Close();
            PanelManager.Instance.Open<MenuRankPanel>();
        }
        private void OnLoadGameClick()
        {
            OpsSoundManager.Instance.PlayBtnClickSound();
            OpsGameManager.Instance.LoadGame();
            PanelManager.Instance.Close();
        }
    }
}

