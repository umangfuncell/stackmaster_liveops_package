using UnityEngine;
using UnityEngine.UI;
using FunCell.GridMerge.GamePlay;
using TMPro;
using FunCell.GridMerge.System;
namespace FunCell.GridMerge.UI
{
    public class JokerShopBtn : MonoBehaviour
    {
        [SerializeField] private int _cost;
        [SerializeField] private Button _btn;
        [SerializeField] private MergeItemLevel _level;
        [SerializeField] private Image _icon_Img;
        [SerializeField] private TMP_Text _cost_Label;
        [SerializeField] private Image _button_Image;
        [SerializeField] private bool _isAlwaysRv;
        [SerializeField] private bool _isRvActive;
        [Space]
        [SerializeField] private Sprite _rvBtn_Sprite;
        [SerializeField] private Sprite _normalBtn_Sprite;
        [SerializeField] private GameObject _withRVObject;
        [SerializeField] private GameObject _withoutRVObject;
        [SerializeField] private GameObject _lockedObject;
        private JokerBtnState _state;
        private bool isTutorial;
        [SerializeField] private GameObject _tutorialObj;
        public void Init(int joker, int cost, MergeItemLevel level, Sprite sprite)
        {
            _cost = cost;
            _icon_Img.sprite = sprite;
            _cost_Label.text = _cost.ToString();
            _level = level;
            HandleState();

            _btn.onClick.AddListener(OnBtnClick);
        }
        private void UpdateInteraction(int joker)
        {
            if (_isAlwaysRv)
            {
                ActiveRvState();
            }
            else
            {
                if (_cost <= joker)
                {
                    ActiveNormalState();
                }
                else
                {
                    ActiveRvState();
                }
            }
        }
        public void Reset()
        {
            _btn.onClick.RemoveListener(OnBtnClick);
        }
        private void OnBtnClick()
        {
            OpsSoundManager.Instance.PlayBtnClickSound();

            if (isTutorial)
            {
                OpsGameManager.Instance.ChangeBlock(_level, 0);
                PanelManager.Instance.Open<GamePanel>();
                _tutorialObj.SetActive(false);
                return;
            }
            if (_state == JokerBtnState.Locked) return;
            if (_isRvActive)
            {
                OpsAdManager.Instance.ShowRewardedAd(() =>
                {
                    OpsGameManager.Instance.ChangeBlock(_level, 0);
                    PanelManager.Instance.Open<GamePanel>();
                }, null, "liveops:unlock_planet_" + _level);
            }
            else
            {
                OpsGameManager.Instance.ChangeBlock(_level, _cost);
                PanelManager.Instance.Open<GamePanel>();
            }
        }
        private void ActiveRvState()
        {
            _isRvActive = true;
            _withRVObject.SetActive(true);
            _withoutRVObject.SetActive(false);
            _lockedObject.SetActive(false);
            _button_Image.sprite = _rvBtn_Sprite;
            _icon_Img.gameObject.SetActive(true);
        }
        private void ActiveNormalState()
        {
            _isRvActive = false;
            _withRVObject.SetActive(false);
            _withoutRVObject.SetActive(true);
            _lockedObject.SetActive(false);
            _button_Image.sprite = _normalBtn_Sprite;
            _icon_Img.gameObject.SetActive(true);
        }
        public void StartTutorial()
        {
            GetComponent<Button>().interactable = true;
            isTutorial = true;
            _button_Image.sprite = _rvBtn_Sprite;
            _withRVObject.SetActive(false);
            _withoutRVObject.SetActive(false);
            _lockedObject.SetActive(false);
            _tutorialObj.SetActive(true);
            _icon_Img.gameObject.SetActive(true);
        }
        public void EndTutorial()
        {
            isTutorial = false;
            _tutorialObj.SetActive(false);
            HandleState();
        }
        public void HandleState()
        {
            Debug.Log($"DataManager.UnlockedItemLevel:{OpsDataManager.UnlockedItemLevel},level{_level}");
            if (OpsDataManager.UnlockedItemLevel < _level)
            {
                _state = JokerBtnState.Locked;
                _lockedObject.SetActive(true);
                _withRVObject.SetActive(false);
                _withoutRVObject.SetActive(false);
                _icon_Img.gameObject.SetActive(false);
                _button_Image.sprite = _rvBtn_Sprite;
            }
            else
            {
                _state = JokerBtnState.Normal;
            }

            if (_state != JokerBtnState.Locked)
            {
                UpdateInteraction(OpsDataManager.Joker);
            }
        }

        private enum JokerBtnState
        {
            Locked,
            Normal
        }
    }
}

