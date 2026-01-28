using UnityEngine;
using System;
using FunCell.GridMerge.GamePlay;
using Newtonsoft.Json;
using FunCell.Helper;
using UnityEngine.Rendering;
using FunCell.GridMerge.UI;
namespace FunCell.GridMerge.System
{
    public class OpsDataManager
    {
        private static int _score;
        public static int Score => _score;

        private static int _highScore;
        public static int HighScore => _highScore;

        private static int _joker;
        public static int Joker => _joker;

        private static int _nextJokerScore;
        public static int NextJokerScore => _nextJokerScore;
        private int _startPointer = 0;

        private static MergeItemLevel _unlockedItemLevel;
        public static MergeItemLevel UnlockedItemLevel => _unlockedItemLevel;

        private static SettingData _settingData;
        public static SettingData SettingData => _settingData;

        private static int _levelCount;
        public static int LevelCount => _levelCount;

        #region Action
        public static event Action<int> OnScoreChange;
        public static event Action<int> OnHighScoreChange;
        public static event Action<int> OnJokerChange;
        public static event Action<SettingData> OnSettingChange;
        public static event Action<MergeItemLevel> OnNewItemUnlock;

        #endregion

        public void Init()
        {
            _joker = 0;
            _highScore = LoadHighScore();
            _settingData = LoadSettingData();
            _levelCount = LoadLevelCount();
            _unlockedItemLevel = MergeItemLevel.Level3;
        }

        public void ResetScore()
        {
            _score = 0;
            _joker = 0;
            _startPointer = 0;
            _unlockedItemLevel = MergeItemLevel.Level3;
            _nextJokerScore = Star.GetNextStarTarget(_startPointer);
            _highScore = LoadHighScore();
        }
        public void AddScore(int value)
        {
            _score += value;
            if (_score >= _highScore)
            {
                _highScore = _score;
                OnHighScoreChange?.Invoke(_highScore);
                SaveHighScore();
            }
            CalculateJoker();
            OnScoreChange?.Invoke(_score);
            OpsGameManager.Instance.gamePanel._bestScore_Label.text = _highScore.ToString();
            // OpsDataManager.OnHighScoreChange += OnHighScoreChange;
        }
        public bool CheckAndUseJoker(int cost)
        {
            if (_joker >= cost)
            {
                _joker -= cost;
                OnJokerChange?.Invoke(_joker);
                return true;
            }
            return false;
        }
        private void CalculateJoker()
        {
            if (_score >= _nextJokerScore)
            {
                _startPointer++;
                _nextJokerScore = Star.GetNextStarTarget(_startPointer);
                _joker++;
                //Umang_P
                // AudioController.Instance.PlayStarSfx();
                OpsAnalyticsManager.Instance.LogEvent("star:earned");
                OnJokerChange?.Invoke(_joker);
            }
        }

        public void SaveGame(GameDataClass gameDataClass)
        {
            gameDataClass.score = Score;
            gameDataClass.joker = Joker;
            gameDataClass.startPointer = _startPointer;
            gameDataClass.unlockedItemLevel = UnlockedItemLevel;

            string saveString = JsonConvert.SerializeObject(gameDataClass);
            PlayerPrefs.SetString("GameData", saveString);
        }
        public GameDataClass Loadgame()
        {
            string saveString = PlayerPrefs.GetString("GameData", null);
            GameDataClass gameDataClass = JsonConvert.DeserializeObject<GameDataClass>(saveString);
            _score = gameDataClass.score;
            _joker = gameDataClass.joker;
            _startPointer = gameDataClass.startPointer;
            _unlockedItemLevel = gameDataClass.unlockedItemLevel;
            //_nextJokerScore = ((_score / 500) * 500) + 500;
            _nextJokerScore = Star.GetNextStarTarget(_startPointer);
            return gameDataClass;
        }

        //use when you need to clean saved data
        public void ClearGame()
        {
            PlayerPrefs.DeleteKey("GameData");
        }

        public static bool CanContinueGame()
        {
            return PlayerPrefs.HasKey("GameData");
        }

        private void SaveHighScore()
        {
            PlayerPrefs.SetInt("HighScore", _highScore);
        }
        private int LoadHighScore()
        {
            return PlayerPrefs.GetInt("HighScore", 0);
        }
        public void SaveSettingData(SettingData data)
        {
            _settingData = data;
            OnSettingChange(_settingData);
            string saveString = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString("SettingData", saveString);
        }
        private SettingData LoadSettingData()
        {
            string saveString = PlayerPrefs.GetString("SettingData", "");
            SettingData data = new SettingData();
            if (!string.IsNullOrEmpty(saveString))
            {
                data = JsonConvert.DeserializeObject<SettingData>(saveString);
            }
            return data;
        }
        public void UnLockNewItemLevel(MergeItemLevel mergeItemLevel)
        {
            Debug.Log("merge item level" + mergeItemLevel);
            _unlockedItemLevel = mergeItemLevel;
            OnNewItemUnlock?.Invoke(_unlockedItemLevel);
        }
        public void IncreseLevelCount()
        {
            _levelCount++;
            SaveLevelCount(_levelCount);
        }
        private void SaveLevelCount(int count)
        {
            PlayerPrefs.SetInt("LevelCount", count);
        }
        private int LoadLevelCount()
        {
            return PlayerPrefs.GetInt("LevelCount", 1);
        }

        public bool CanPlayTutorial()
        {
            return !PlayerPrefs.HasKey("Tutorial");
        }
        public void SaveTutotrial()
        {
            PlayerPrefs.SetString("Tutorial", "");
        }
        public bool IsOpsTutorialFinished
        {
            get
            {
                return PlayerPrefs.GetInt("IsOpsTutorialFinished", 0) == 0 ? false : true;
            }
            set
            {
                PlayerPrefs.SetInt("IsOpsTutorialFinished", value == false ? 0 : 1);
            }
        }
    }

    public class GameDataClass
    {
        public MergeItemLevel[,] gridItems;
        public MergeItemLevel[] spawnerItems;
        public NodeState[] rvNodeStatesList;
        public MergeItemLevel unlockedItemLevel = MergeItemLevel.Level3;
        public int rvNodeCounter = 0;
        public int score;
        public int joker;
        public int startPointer;
    }
    public class SettingData
    {
        public bool isMusicOn = true;
        public bool isSfxOn = true;
        public bool isHapticOn = true;
    }
    public static class Star
    {
        public static int[] StarList = new int[] { 500, 1000, 1750, 2500 };

        public static int GetNextStarTarget(int pointer)
        {
            if (pointer >= StarList.Length)
            {
                return StarList[StarList.Length - 1] + (((pointer - StarList.Length) + 1) * 1000);
            }
            else
            {
                return StarList[pointer];
            }
        }
    }
}

