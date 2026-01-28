using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using FunCell.GridMerge.GamePlay;
using FunCell.GridMerge.System;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor;
namespace FunCell.GridMerge.UI
{
    public class GamePanel : Panel
    {
        [SerializeField] public TMP_Text _score_Lable, _bestScore_Label;
        [SerializeField] private TMP_Text starTxt;
        [SerializeField] private Button _reStart_Btn;
        [SerializeField] private Button _BackBtn;

        [SerializeField] private Image _levelFill_Img;

        [SerializeField] private StarBtnHadler _starBtnHadler;
        [SerializeField] private RewardWidgets _rewardWidgets;
        // [SerializeField] private List<JokerShopBtn> _buttons;
        [SerializeField] private Transform _bestScoreContainer;
        [SerializeField] private Transform _starContainer;
        [SerializeField] private RewardClaimSubPanel _rewardClaimSubPanel;
        private Tutorial _tutorial;
        float tutorialTime = 10f;
        public Transform _ui_hand;
        public int HintTutorialCount = 0;
        public IEnumerator HintTutorialIEnumerator;
        public LiveLeaderBoard _liveLeaderBoard;

        void Start()
        {
            if (OpsGameManager.OpsDataManager.IsOpsTutorialFinished == true)
            {
                HintTutorialIEnumerator = ChecknShowHandTutorial();
                StartCoroutine(HintTutorialIEnumerator);
            }
        }

        public IEnumerator ChecknShowHandTutorial()
        {
            yield return new WaitForSeconds(tutorialTime);
            if (HintTutorialCount < 2)
            {
                CheckEmptySpace();
            }
        }

        public void StopHintTutorialIEnumerator()
        {
            if (HintTutorialIEnumerator != null)
            {
                StopCoroutine(HintTutorialIEnumerator);
            }
            if (OpsGameManager.OpsDataManager.IsOpsTutorialFinished == true)
            {
                HintTutorialIEnumerator = ChecknShowHandTutorial();
                StartCoroutine(HintTutorialIEnumerator);
            }
        }

        public void CheckEmptySpace()
        {
            MergeGridNode node = OpsGameManager.Instance.GetEmptyNode();
            if (node != null)
            {
                _ui_hand.gameObject.SetActive(true);
                _ui_hand.transform.LeanMove(Camera.main.WorldToScreenPoint(node.GetSpawnPoint()), 0.2f);
                HintTutorialCount++;
            }
        }

        public override void Open()
        {
            base.Open();

            OpsDataManager.OnScoreChange += OnScoreChange;
            OpsDataManager.OnJokerChange += OnStarChange;
            OpsDataManager.OnNewItemUnlock += OnNewItemUnlock;

            _reStart_Btn.onClick.AddListener(OnReStartClick);
            _BackBtn.onClick.AddListener(OnBackClick);

            OnScoreChange(OpsDataManager.Score);
            OnStarChange(OpsDataManager.Joker);
            OnHighScoreChange(OpsDataManager.HighScore);
            OnStarChange(OpsDataManager.Joker);

            _starBtnHadler.InitJokerButtones();
            _rewardWidgets.Open();
            _liveLeaderBoard.Open();

            if (_rewardClaimSubPanel.isActiveAndEnabled)
            {
                LeanTween.delayedCall(0.1f, () => OpsGameManager.Instance.ChangeStateTo(GameState.Pause));
            }
            else
            {
                LeanTween.delayedCall(0.1f, () => OpsGameManager.Instance.ChangeStateTo(GameState.Resume));
            }
        }
        public override void Close()
        {
            base.Close();

            OpsDataManager.OnNewItemUnlock -= OnNewItemUnlock;
            OpsDataManager.OnScoreChange -= OnScoreChange;
            OpsDataManager.OnJokerChange -= OnStarChange;

            // OpsDataManager.OnHighScoreChange -= OnHighScoreChange;

            _starBtnHadler.ResetJokerButtones();
            _liveLeaderBoard.Close();
            _rewardWidgets.Close();
            _reStart_Btn.onClick.RemoveListener(OnReStartClick);
            _BackBtn.onClick.RemoveListener(OnBackClick);
            OpsGameManager.Instance.ChangeStateTo(GameState.Pause);
        }

        private void OnScoreChange(int score)
        {
            _score_Lable.text = score.ToString();
            // _levelFill_Img.fillAmount = (float)score / OpsDataManager.NextJokerScore;
        }
        private void OnHighScoreChange(int score)
        {
            _bestScore_Label.text = score.ToString();
            // _levelFill_Img.fillAmount = (float)score / OpsDataManager.NextJokerScore;
        }
        private void OnStarChange(int starNo)
        {
            starTxt.text = starNo.ToString();
        }
        private void OnReStartClick()
        {
            OpsSoundManager.Instance.PlayBtnClickSound();
            PanelManager.Instance.Close();
            OpsGameManager.Instance.UnLoadGame();
        }

        private void OnBackClick()
        {
            OpsAnalyticsManager.Instance.LogEvent("liveops:back_clicked");
            OpsSoundManager.Instance.PlayBtnClickSound();
            PanelManager.Instance.Open<Warning>();
        }

        public void StartTutorial(Tutorial tutorial)
        {
            _tutorial = tutorial;
            // _setting_Btn.gameObject.SetActive(false);
            _bestScoreContainer.gameObject.SetActive(false);
            _starContainer.gameObject.SetActive(false);
            _starBtnHadler.StartTutorial();
        }

        public void EndTutorial()
        {
            _tutorial = null;
            // _setting_Btn.gameObject.SetActive(true);
            _bestScoreContainer.gameObject.SetActive(true);
            _starContainer.gameObject.SetActive(true);
            // _joker_Btn.interactable = true;
            _starBtnHadler.EndTutorial();
        }
        public void OnNewItemUnlock(MergeItemLevel mergeItemLevel)
        {
            PanelManager.Instance.Open<NewItemUnlockPopup>();
        }
        public void OnRewardGrant(OpsRewardChest chest)
        {
            _rewardClaimSubPanel.Open(chest);
        }
    }
}

