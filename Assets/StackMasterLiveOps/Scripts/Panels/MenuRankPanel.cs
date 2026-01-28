using UnityEngine;
using UnityEngine.UI;
using FunCell.GridMerge.GamePlay;
using FunCell.GridMerge.System;
namespace FunCell.GridMerge.UI
{

    public class MenuRankPanel : Panel
    {
        [SerializeField] private Button _start_Btn;
        [SerializeField] private Button _loadGame_Btn;
        public override void Open()
        {
            base.Open();

            _start_Btn.onClick.AddListener(OnStartClick);
            _loadGame_Btn.onClick.AddListener(OnLoadGameClick);

            _loadGame_Btn.interactable = OpsDataManager.CanContinueGame();
        }
        public override void Close()
        {
            base.Close();

            _start_Btn.onClick.RemoveListener(OnStartClick);
            _loadGame_Btn.onClick.RemoveListener(OnLoadGameClick);
        }
        private void OnStartClick()
        {
            OpsAnalyticsManager.Instance.LogEvent("liveops:tap_to_continue");
            OpsSoundManager.Instance.PlayBtnClickSound();
            OpsGameManager.Instance.NewGame();
            PanelManager.Instance.Close();
            if (Tutorial.Instance != null)
            {
                Tutorial.Instance.SpawnTutorialItem();
            }
        }
        private void OnLoadGameClick()
        {
            OpsSoundManager.Instance.PlayBtnClickSound();
            OpsGameManager.Instance.LoadGame();
            PanelManager.Instance.Close();
        }
    }
}

