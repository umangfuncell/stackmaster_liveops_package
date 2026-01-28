using UnityEngine;
using UnityEngine.UI;
using FunCell.GridMerge.GamePlay;
using TMPro;
using FunCell.GridMerge.System;
namespace FunCell.GridMerge.UI
{
    public class GameOverPanel : Panel
    {
        [SerializeField] private Button _reStart_Btn;
        [SerializeField] private Button _main_Btn;
        [SerializeField] private TMP_Text _score_Lable;
        public override void Open()
        {
            base.Open();
            _reStart_Btn.onClick.AddListener(OnReStartClick);
            _main_Btn.onClick.AddListener(OnMainClick);
            _score_Lable.text = OpsDataManager.Score.ToString();
        }
        public override void Close()
        {
            base.Close();
            _reStart_Btn.onClick.RemoveListener(OnReStartClick);
        }
        private void OnReStartClick()
        {
            PanelManager.Instance.Close();
            OpsGameManager.Instance.NewGame();
        }
        private void OnMainClick()
        {
            //umang
            //GameManager.Instance.DestoryLiveOpsObj();
        }
    }
}
