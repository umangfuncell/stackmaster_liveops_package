using UnityEngine;
using UnityEngine.UI;
using FunCell.GridMerge.GamePlay;
using TMPro;
using FunCell.GridMerge.System;
namespace FunCell.GridMerge.UI
{
    public class Warning : Panel
    {
        [SerializeField] private Button yesBtn;
        [SerializeField] private Button noBtn;
        [SerializeField] private Button RVBtn;
        [SerializeField] private TMP_Text EnergyText;
        [SerializeField] private TMP_Text RVText;
        float currEnergy = 0;
        float multiplier = 5;
        public Image EnergyImg;
        public Sprite world1Energy, world2Energy, marsEnergy;
        public override void Open()
        {
            base.Open();
            yesBtn.onClick.AddListener(OnYesClick);
            noBtn.onClick.AddListener(OnNoClick);
            RVBtn.onClick.AddListener(OnRVClick);
            // _main_Btn.onClick.AddListener(OnMainClick);
            // _score_Lable.text = OpsDataManager.Score.ToString();
            //umang
            // currEnergy = OpsDataManager.Score * DataManager.Instance.EnergyGeneratedPerSec;
            // EnergyText.text = DataManager.Instance.GetString(currEnergy);
            // RVText.text = DataManager.Instance.GetString(currEnergy * multiplier);

            // if (DataManager.Instance.LastPlanetActive == 0)
            // {
            //     EnergyImg.sprite = world1Energy;
            // }
            // else if (DataManager.Instance.LastPlanetActive == 2)
            // {
            //     EnergyImg.sprite = world2Energy;
            // }
            // else
            // {
            //     EnergyImg.sprite = marsEnergy;
            // }

        }
        public override void Close()
        {
            base.Close();
            yesBtn.onClick.RemoveListener(OnYesClick);
            noBtn.onClick.RemoveListener(OnNoClick);
            RVBtn.onClick.RemoveListener(OnRVClick);
        }
        private void OnYesClick()
        {
            //umang
            //UIManager.Instance.CoffeeN2xCanvas.sortingOrder = 0;
            // Debug.LogError("OnYesClick");
            //ParticleManager.Instance.GenerateEnergyGainParticle(this.transform);
            // if (DataManager.Instance.LastPlanetActive == 0)
            // {
            //     DataManager.Instance.CurrentEnergy += currEnergy;
            //     // AnalyticsManager.Instance.LogBasicGAEvent("levelup:world_1:level_close");
            //     // AnalyticsManager.Instance.LogLevelUpEvent("world_1:level_"+LevelUp.Instance.world1Level);
            // }
            // else if (DataManager.Instance.LastPlanetActive == 2)
            // {
            //     DataManager.Instance.CurrentEnergyWorld2 += currEnergy;
            //     // AnalyticsManager.Instance.LogBasicGAEvent("levelup:world_2:level_close");
            //     // AnalyticsManager.Instance.LogLevelUpEvent("world_2:level_"+LevelUp.Instance.world2Level);
            // }
            // else
            // {
            //     DataManager.Instance.CurrentEnergyMars += currEnergy;
            //     // AnalyticsManager.Instance.LogBasicGAEvent("levelup:mars:level_close");
            //     // AnalyticsManager.Instance.LogLevelUpEvent("mars:level_"+LevelUp.Instance.marsLevel);
            // }

            OpsAnalyticsManager.Instance.LogEvent("liveops:back_yes_clicked");
            OpsSoundManager.Instance.PlayBtnClickSound();

            //umang
            //GameManager.Instance.DestoryLiveOpsObj();
            //TutorialManager.Instance.DisableMessage();
        }
        private void OnNoClick()
        {
            // Debug.LogError("OnNoClick");
            OpsAnalyticsManager.Instance.LogEvent("liveops:back_no_clicked");
            OpsSoundManager.Instance.PlayBtnClickSound();
            PanelManager.Instance.Open<GamePanel>();
        }
        private void OnRVClick()
        {
            // Debug.LogError("OnRVClick");
            OpsAdManager.Instance.ShowRewardedAd(CallOnRVClick, null, "liveops:back_rv_clicked");
            // AnalyticsManager.Instance.LogBasicGAEvent("liveops:back_rv_clicked");
            OpsSoundManager.Instance.PlayBtnClickSound();
        }

        private void CallOnRVClick()
        {
            //umang
            //UIManager.Instance.CoffeeN2xCanvas.sortingOrder = 0;
            // AnalyticsManager.Instance.LogBasicGAEvent("liveops:back_rv_clicked");

            //umang
            //ParticleManager.Instance.GenerateEnergyGainParticle(this.transform);
            // if (DataManager.Instance.LastPlanetActive == 0)
            // {
            //     DataManager.Instance.CurrentEnergy += (currEnergy * multiplier);
            //     // AnalyticsManager.Instance.LogBasicGAEvent("levelup:world_1:level_close");
            //     // AnalyticsManager.Instance.LogLevelUpEvent("world_1:level_"+LevelUp.Instance.world1Level);
            // }
            // else if (DataManager.Instance.LastPlanetActive == 2)
            // {
            //     DataManager.Instance.CurrentEnergyWorld2 += (currEnergy * multiplier);
            //     // AnalyticsManager.Instance.LogBasicGAEvent("levelup:world_2:level_close");
            //     // AnalyticsManager.Instance.LogLevelUpEvent("world_2:level_"+LevelUp.Instance.world2Level);
            // }
            // else
            // {
            //     DataManager.Instance.CurrentEnergyMars += (currEnergy * multiplier);
            //     // AnalyticsManager.Instance.LogBasicGAEvent("levelup:mars:level_close");
            //     // AnalyticsManager.Instance.LogLevelUpEvent("mars:level_"+LevelUp.Instance.marsLevel);
            // }
            // GameManager.Instance.DestoryLiveOpsObj();
            // TutorialManager.Instance.DisableMessage();
        }
    }
}
