using UnityEngine;
using UnityEngine.UI;
using FunCell.GridMerge.GamePlay;
using TMPro;
using FunCell.GridMerge.System;
namespace FunCell.GridMerge.UI
{
    public class LiveOpsPopup : Panel
    {

        [SerializeField] private Button CollectByDiamondBtn, CollectRVVideoBtn, CloseBtn;
        [SerializeField] private Text BaseEnergyText, CollectByDiamondText, CollectRVVideoText, ScoreTxt, BestScoreTxt;

        float baseEnergy, baseDiamondEnergy, baseVideoRVEnergy = 0;
        public Text NotEnoughDiamondsTxt;

        public Image EnergyImg, DiamondEnegyImg, RVEnegyImg;
        public Sprite w1_EnergySprite, w2_EnergySprite, mars_EnergySprite;

        public void GenerateNotEnoughDiamondsTxt()
        {
            GameObject g = Instantiate(NotEnoughDiamondsTxt.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
            Text g_Txt = g.GetComponent<Text>();
            g.transform.parent = NotEnoughDiamondsTxt.transform.parent;
            g.transform.localPosition = NotEnoughDiamondsTxt.transform.localPosition;

            LeanTween.moveLocalY(g, g.transform.localPosition.y + 100, 0.5f).setEase(LeanTweenType.easeOutExpo);


            LeanTween.value(gameObject, (float val) =>
            {
                Color tmpColor = g_Txt.color;
                tmpColor.a = val;
                g_Txt.color = tmpColor;
            }, 0f, 1f, 0.5f).setEase(LeanTweenType.easeOutExpo).setOnComplete(() =>
            {
                LeanTween.value(gameObject, (float val) =>
                {
                    Color tmpColor = g_Txt.color;
                    tmpColor.a = val;
                    g_Txt.color = tmpColor;
                }, 1f, 0f, 0.2f).setEase(LeanTweenType.easeInExpo).setOnComplete(() =>
                {
                    Destroy(g);
                });
            });

            // LeanTween.alpha(g.gameObject, 1, 1).setEase(LeanTweenType.easeOutExpo).setOnComplete(() =>
            // {
            //     LeanTween.alpha(g.gameObject, 0, 0.5f).setEase(LeanTweenType.easeInExpo).setOnComplete(() =>
            //     {
            //         Destroy(g);
            //     });
            // });
        }
        public override void Open()
        {
            base.Open();
            OpsAnalyticsManager.Instance.LogEvent("liveops:reward_popup");
            CollectByDiamondBtn.onClick.AddListener(CallCollectByDiamond);
            CollectRVVideoBtn.onClick.AddListener(CallCollectRVVideo);
            CloseBtn.onClick.AddListener(CallCollectByFree);

            //umang
            // if (DataManager.Instance.LastPlanetActive == 0)
            // {
            //     EnergyImg.sprite = w1_EnergySprite;
            //     DiamondEnegyImg.sprite = w1_EnergySprite;
            //     RVEnegyImg.sprite = w1_EnergySprite;

            //     baseEnergy = (OpsDataManager.Score * DataManager.Instance.EnergyGeneratedPerSec) / 10;
            //     baseDiamondEnergy = ((OpsDataManager.Score * DataManager.Instance.EnergyGeneratedPerSec) / 10) * 5;
            //     baseVideoRVEnergy = ((OpsDataManager.Score * DataManager.Instance.EnergyGeneratedPerSec) / 10) * 5;

            //     // Debug.LogError("EnergyGeneratedPerSec = " + DataManager.Instance.EnergyGeneratedPerSec);
            // }
            // else if (DataManager.Instance.LastPlanetActive == 2)
            // {
            //     EnergyImg.sprite = w2_EnergySprite;
            //     DiamondEnegyImg.sprite = w2_EnergySprite;
            //     RVEnegyImg.sprite = w2_EnergySprite;

            //     baseEnergy = (OpsDataManager.Score * DataManager.Instance.EnergyGeneratedPerSecWorld2) / 10;
            //     baseDiamondEnergy = ((OpsDataManager.Score * DataManager.Instance.EnergyGeneratedPerSecWorld2) / 10) * 5;
            //     baseVideoRVEnergy = ((OpsDataManager.Score * DataManager.Instance.EnergyGeneratedPerSecWorld2) / 10) * 5;

            //     // Debug.LogError("EnergyGeneratedPerSecWorld2 = " + DataManager.Instance.EnergyGeneratedPerSecWorld2);
            // }
            // else
            // {

            //     EnergyImg.sprite = mars_EnergySprite;
            //     DiamondEnegyImg.sprite = mars_EnergySprite;
            //     RVEnegyImg.sprite = mars_EnergySprite;

            //     baseEnergy = (OpsDataManager.Score * DataManager.Instance.EnergyGeneratedPerSecMars) / 10;
            //     baseDiamondEnergy = ((OpsDataManager.Score * DataManager.Instance.EnergyGeneratedPerSecMars) / 10) * 5;
            //     baseVideoRVEnergy = ((OpsDataManager.Score * DataManager.Instance.EnergyGeneratedPerSecMars) / 10) * 5;

            //     // Debug.LogError("EnergyGeneratedPerSecMars = " + DataManager.Instance.EnergyGeneratedPerSecMars);
            // }

            // // Debug.LogError("baseEnergy" + baseEnergy);
            // // Debug.LogError("baseDiamondEnergy" + baseDiamondEnergy);
            // // Debug.LogError("baseVideoRVEnergy" + baseVideoRVEnergy);

            // ScoreTxt.text = "You Scored : " + OpsDataManager.Score.ToString();
            // BestScoreTxt.text = "Best : " + OpsDataManager.HighScore.ToString();

            // BaseEnergyText.text = DataManager.Instance.GetString(baseEnergy, true);
            // CollectByDiamondText.text = DataManager.Instance.GetString(baseDiamondEnergy, true);
            // CollectRVVideoText.text = DataManager.Instance.GetString(baseVideoRVEnergy, true);
        }
        public override void Close()
        {
            base.Close();
            CollectByDiamondBtn.onClick.RemoveListener(CallCollectByDiamond);
            CollectRVVideoBtn.onClick.RemoveListener(CallCollectRVVideo);
            CloseBtn.onClick.RemoveListener(CallCollectByFree);
        }

        //------- Free ---------
        public void CallCollectByFree()
        {
            OpsSoundManager.Instance.PlayBtnClickSound();
            CallCollectByFree_Complete();
        }

        void CallCollectByFree_Complete()
        {
            // Debug.LogError("Free...");
            AddEnergy(baseEnergy);
            //ParticleManager.Instance.GenerateEnergyGainParticle(CloseBtn.transform);
            //GameManager.Instance.DestoryLiveOpsObj();
            //TutorialManager.Instance.DisableMessage();
            Close();
        }

        //------- Diamonds ---------

        public void CallCollectByDiamond()
        {
            OpsSoundManager.Instance.PlayBtnClickSound();
            CollectByDiamond_Complete();
        }

        void CollectByDiamond_Complete()
        {
            // Debug.LogError("Gems Complete...");
            // if (DataManager.Instance.PremiumCurrency >= 5)
            // {
            //     OpsAnalyticsManager.Instance.LogEvent("liveops:gems_energy");
            //     //DataManager.Instance.PremiumCurrency -= 5;
            //     AddEnergy(baseDiamondEnergy);
            //     ParticleManager.Instance.GenerateEnergyGainParticle(CollectByDiamondBtn.transform);
            //     GameManager.Instance.DestoryLiveOpsObj();
            //     TutorialManager.Instance.DisableMessage();
            //     Close();
            // }
            // else
            // {
            //     // UIManager.Instance.OpenStoreScreen();
            //     GenerateNotEnoughDiamondsTxt();
            // }
        }

        //------- Video RV ---------

        public void CallCollectRVVideo()
        {
            OpsSoundManager.Instance.PlayBtnClickSound();
            OpsAdManager.Instance.ShowRewardedAd(CallCollectRVVideo_Complete, null, "liveops:rv_energy");
        }

        void CallCollectRVVideo_Complete()
        {
            // Debug.LogError("Video RV...");
            AddEnergy(baseVideoRVEnergy);
            //umang
            // ParticleManager.Instance.GenerateEnergyGainParticle(CollectRVVideoBtn.transform);
            // GameManager.Instance.DestoryLiveOpsObj();
            // TutorialManager.Instance.DisableMessage();
            Close();
        }
        public void AddEnergy(float tmpEnergy)
        {
            // umang
            // if (DataManager.Instance.LastPlanetActive == 0)
            // {
            //     DataManager.Instance.CurrentEnergy += tmpEnergy;
            // }
            // else if (DataManager.Instance.LastPlanetActive == 1)
            // {
            //     DataManager.Instance.CurrentEnergyMars += tmpEnergy;
            // }
            // else
            // {
            //     DataManager.Instance.CurrentEnergyWorld2 += tmpEnergy;
            // }
        }
    }

}
