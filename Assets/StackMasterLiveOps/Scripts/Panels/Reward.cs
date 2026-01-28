using UnityEngine;
using UnityEngine.UI;
using FunCell.GridMerge.GamePlay;
using TMPro;
using FunCell.GridMerge.System;
namespace FunCell.GridMerge.UI
{
    public class Reward : Panel
    {
        // [SerializeField] private Button _main_Btn;
        [SerializeField] private Text BextScoreTxt;
        [SerializeField] private Text ScoreTxt;
        [SerializeField] private Text EnergyTxt;
        [SerializeField] private Button WatchVideo5Sec;
        [SerializeField] private Text WatchVideo5SecTxt;
        [SerializeField] private Button WatchVideo30Sec;
        [SerializeField] private Text WatchVideo30SecTxt;
        [SerializeField] private Button CollectWithDiamonds;
        [SerializeField] private Text CollectWithDiamondsTxt;
        [SerializeField] private Button RewardClose;
        public override void Open()
        {
            base.Open();
            WatchVideo5Sec.onClick.AddListener(OnWatchVideo5SecClick);
            WatchVideo30Sec.onClick.AddListener(OnWatchVideo30SecClick);
            CollectWithDiamonds.onClick.AddListener(OnCollectWithDiamondsClick);
            RewardClose.onClick.AddListener(OnRewardCloseClick);
            // _main_Btn.onClick.AddListener(OnMainClick);
            // _score_Lable.text = OpsDataManager.Score.ToString();
        }
        public override void Close()
        {
            base.Close();
            WatchVideo5Sec.onClick.RemoveListener(OnWatchVideo5SecClick);
            WatchVideo30Sec.onClick.RemoveListener(OnWatchVideo30SecClick);
            CollectWithDiamonds.onClick.RemoveListener(OnCollectWithDiamondsClick);
            RewardClose.onClick.RemoveListener(OnRewardCloseClick);
        }
        private void OnWatchVideo5SecClick()
        {
            // Debug.LogError("OnWatchVideo5SecClick");
            OpsAdManager.Instance.ShowRewardedAd(FinishOnWatchVideo5Sec, null, "OfflineVideo");
        }

        void FinishOnWatchVideo5Sec()
        {
            PanelManager.Instance.Close();

            //umang
            // GameManager.Instance.DestoryLiveOpsObj();
            // TutorialManager.Instance.DisableMessage();

            // DataManager.Instance.CurrentEnergy += (GameManager.Instance.offlineEnergy * RemoteParameters.Instance.Offline_Watch_Multiplier);
            // MissionManager.Instance.CheckForEarnMissionAndAdd((offlineEnergy * RemoteParameters.Instance.Offline_Watch_Multiplier));
            // IsDirectCloseFromOffline = false;
            // CloseOfflineScreen();
            // OpenPopup(RemoteParameters.Instance.Offline_Watch_Multiplier.ToString() + "X More " + LocalizationManager.Instance.GetText("energy"), LocalizationManager.Instance.GetText("you_have_got") + " +" + DataManager.Instance.GetString(offlineEnergy + (offlineEnergy * RemoteParameters.Instance.Offline_Watch_Multiplier)) + " " + LocalizationManager.Instance.GetText("energy"), ShowGainEffect);
        }

        private void OnWatchVideo30SecClick()
        {
            // Debug.LogError("OnWatchVideo30SecClick");
            OpsAdManager.Instance.ShowRewardedAd(FinishOnWatchVideo30Sec, null, "OfflineVideo");
        }
        void FinishOnWatchVideo30Sec()
        {
            PanelManager.Instance.Close();

            //umang
            // GameManager.Instance.DestoryLiveOpsObj();
            // TutorialManager.Instance.DisableMessage();

            // DataManager.Instance.CurrentEnergy += (GameManager.Instance.offlineEnergy * RemoteParameters.Instance.Offline_Watch_Multiplier);
            // MissionManager.Instance.CheckForEarnMissionAndAdd((offlineEnergy * RemoteParameters.Instance.Offline_Watch_Multiplier));
            // IsDirectCloseFromOffline = false;
            // CloseOfflineScreen();
            // OpenPopup(RemoteParameters.Instance.Offline_Watch_Multiplier.ToString() + "X More " + LocalizationManager.Instance.GetText("energy"), LocalizationManager.Instance.GetText("you_have_got") + " +" + DataManager.Instance.GetString(offlineEnergy + (offlineEnergy * RemoteParameters.Instance.Offline_Watch_Multiplier)) + " " + LocalizationManager.Instance.GetText("energy"), ShowGainEffect);
        }

        private void OnCollectWithDiamondsClick()
        {
            // Debug.LogError("OnCollectWithDiamondsClick");
            OpsAdManager.Instance.ShowRewardedAd(FinishOnCollectWithDiamonds, null, "OfflineVideo");
        }
        void FinishOnCollectWithDiamonds()
        {
            PanelManager.Instance.Close();

            //umang
            // GameManager.Instance.DestoryLiveOpsObj();
            // TutorialManager.Instance.DisableMessage();

            // DataManager.Instance.CurrentEnergy += (GameManager.Instance.offlineEnergy * RemoteParameters.Instance.Offline_Watch_Multiplier);
            // MissionManager.Instance.CheckForEarnMissionAndAdd((offlineEnergy * RemoteParameters.Instance.Offline_Watch_Multiplier));
            // IsDirectCloseFromOffline = false;
            // CloseOfflineScreen();
            // OpenPopup(RemoteParameters.Instance.Offline_Watch_Multiplier.ToString() + "X More " + LocalizationManager.Instance.GetText("energy"), LocalizationManager.Instance.GetText("you_have_got") + " +" + DataManager.Instance.GetString(offlineEnergy + (offlineEnergy * RemoteParameters.Instance.Offline_Watch_Multiplier)) + " " + LocalizationManager.Instance.GetText("energy"), ShowGainEffect);
        }

        private void OnRewardCloseClick()
        {
            // Debug.LogError("OnRewardCloseClick");
            OpsAdManager.Instance.ShowRewardedAd(FinishOnRewardClose, null, "OfflineVideo");
        }

        void FinishOnRewardClose()
        {
            PanelManager.Instance.Close();

            //umang
            // GameManager.Instance.DestoryLiveOpsObj();
            // TutorialManager.Instance.DisableMessage();

            // DataManager.Instance.CurrentEnergy += (GameManager.Instance.offlineEnergy * RemoteParameters.Instance.Offline_Watch_Multiplier);
            // MissionManager.Instance.CheckForEarnMissionAndAdd((offlineEnergy * RemoteParameters.Instance.Offline_Watch_Multiplier));
            // IsDirectCloseFromOffline = false;
            // CloseOfflineScreen();
            // OpenPopup(RemoteParameters.Instance.Offline_Watch_Multiplier.ToString() + "X More " + LocalizationManager.Instance.GetText("energy"), LocalizationManager.Instance.GetText("you_have_got") + " +" + DataManager.Instance.GetString(offlineEnergy + (offlineEnergy * RemoteParameters.Instance.Offline_Watch_Multiplier)) + " " + LocalizationManager.Instance.GetText("energy"), ShowGainEffect);
        }
    }
}
