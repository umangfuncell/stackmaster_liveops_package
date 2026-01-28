using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using FunCell.GridMerge.System;
using System;
using Random = UnityEngine.Random;

namespace FunCell.GridMerge.UI
{
    public class LiveLeaderBoard : MonoBehaviour
    {
        [SerializeField] private int _totalPlayerCount;
        [SerializeField] private int _minScore = 50;
        [SerializeField] private int _maxScore = 1400;

        #region UI Reference
        [Header("UI")]
        [SerializeField] private Button _close_Btn;
        [SerializeField] private TMP_Text _score_Label;
        [SerializeField] private ScrollRect _leaderboardScrollRect;
        [SerializeField] private Transform _contentObj;
        #endregion


        [Header("Spawn Item Prefab")]
        [SerializeField] private LeaderBoardItem _leaderboardItem;
        [SerializeField] private LeaderBoardItem _playerLeaderboardItem;

        [SerializeField] private List<LeaderBoardItem> _topThreePlayerList;
        [SerializeField] private List<ScoreCategory> _scoreCategoryList;

        public int Score, LastScore;

        [SerializeField] Transform viewPanel;

        [Header("VisiblePlayerItem")]
        [SerializeField] private LeaderBoardItem _visiblePlayerItem;
        [SerializeField] private float _startPos;
        [SerializeField] private float _endPos;

        [Space]
        [SerializeField] private float _activeDuration = 5f;
        [SerializeField] private int _scoreIncreaseValue = 200;

        private bool _isViewPanelVisible = false;
        private int leaderboardTweenId;
        private LeaderBoardItem spawnedPlayerItem;
        private Coroutine _scoreUpdateRoutine;
        private Coroutine _hideViewPanelCoroutine;
        private bool isInitialized;

        public void Open()
        {
            if (!isInitialized)
            {
                isInitialized = true;
                ShowLeaderBoard(OpsDataManager.HighScore);
            }

            SetVisiblePlayerItemData();
            ShowPlayerActiveItem();

            _close_Btn.onClick.AddListener(OnCloseBtnClick);
            OpsDataManager.OnHighScoreChange += OnHighScoreChange;
        }
        public void Close()
        {
            _close_Btn.onClick.RemoveListener(OnCloseBtnClick);
            OpsDataManager.OnHighScoreChange -= OnHighScoreChange;
        }
        private void OnHighScoreChange(int highScore)
        {
            if (spawnedPlayerItem != null)
            {
                int index = spawnedPlayerItem.transform.GetSiblingIndex();
                LeaderBoardItem otherPlayerItem = _contentObj.transform.GetChild(index - 1).GetComponent<LeaderBoardItem>();
                spawnedPlayerItem.SetScroe(OpsDataManager.HighScore);
                SetVisiblePlayerItemData();
                if (otherPlayerItem.Score < OpsDataManager.HighScore)
                {
                    HidePlayerActiveItem(() =>
                    {
                        _close_Btn.gameObject.SetActive(false);
                        IncreaseRank(OpsDataManager.HighScore, spawnedPlayerItem.Score);
                    });
                }
            }
            else
            {
                IncreaseRank(OpsDataManager.HighScore, OpsDataManager.Score - 100);
            }
        }
        private void SetCurrentScore(int score)
        {
            // _score_Label.text = score.ToString();
        }

        [ContextMenu("ShowLeaderBoard")]
        public void ShowBoard()
        {
            // LastScore = Score;
            // Score += int.Parse(inputFieldScore.text);
            // ShowLeaderBoard(Score, LastScore);
            IncreaseRank(Score, LastScore);
        }
        public void ShowLeaderBoard(int highScore = 1, int lastGameScore = 0)
        {
            if (!isAnimationComplate) return;
            isAnimationComplate = false;
            ClearLeaderBoard();
            int newRank = GetRankFormRange(highScore);
            int lastRank = GetRankFormRange(lastGameScore);
            InitializeLeaderBoard(highScore, lastGameScore, newRank, lastRank);
        }
        public void InitializeLeaderBoard(int highScore, int lastScore, int newRank, int lastRank, float animationDelay = 1f)
        {
            CalculatePlayersAround(highScore, newRank, out int playersAbovePlayer, out int playersBelowPlayer);

            int rankup = Mathf.Max(0, lastRank - newRank);
            int numberOfBarsToSpawn = 5 + playersAbovePlayer + playersBelowPlayer;


            GenerateScores(highScore, playersAbovePlayer, playersBelowPlayer, rankup, numberOfBarsToSpawn,
                out List<int> higherScores,
                out List<int> lowerScores
            );

            spawnedPlayerItem = SpawnLeaderboardItems(newRank, lastRank, lastScore, playersAbovePlayer, playersBelowPlayer, higherScores, lowerScores, numberOfBarsToSpawn);

            PlayLeaderboardAnimation(spawnedPlayerItem, lastRank, newRank, lastScore, highScore, playersAbovePlayer);
        }
        private void CalculatePlayersAround(int highScore, int newRank, out int playersAbovePlayer, out int playersBelowPlayer)
        {
            playersAbovePlayer = 2;
            playersBelowPlayer = 4;

            if (highScore <= _minScore)
            {
                playersAbovePlayer = 7;
                playersBelowPlayer = 0;
            }
            else if (highScore >= _maxScore)
            {
                playersAbovePlayer = 0;
                playersBelowPlayer = 7;
            }

            if (newRank == 1)
            {
                playersAbovePlayer = 0;
                playersBelowPlayer = 7;
            }
            else if (newRank == 2)
            {
                playersAbovePlayer = 1;
                playersBelowPlayer = 6;
            }
            else if (newRank == 3)
            {
                playersAbovePlayer = 2;
                playersBelowPlayer = 5;
            }
        }
        private float GetCategoryDifficulty(int score)
        {
            foreach (ScoreCategory category in _scoreCategoryList)
            {
                if (score >= category.MinScore && score <= category.MaxScore)
                {
                    float range = category.MaxScore - category.MinScore;
                    float offset = score - category.MinScore;

                    return Mathf.Clamp01(offset / range);
                }
            }

            return 1f;
        }
        private void GenerateScores(int highScore, int playersAbovePlayer, int playersBelowPlayer, int rankup, int numberOfBarsToSpawn,
        out List<int> higherScores, out List<int> lowerScores
        )
        {
            higherScores = new List<int>(playersAbovePlayer);
            lowerScores = new List<int>(rankup + playersBelowPlayer);

            float difficulty = GetCategoryDifficulty(highScore);

            int minGap = Mathf.RoundToInt(Mathf.Lerp(60, 300, difficulty));
            int maxGap = Mathf.RoundToInt(Mathf.Lerp(180, 1400, difficulty));

            int cumulativeGap = 0;

            for (int i = 0; i < playersAbovePlayer; i++)
            {
                int gap = Random.Range(minGap, maxGap);
                cumulativeGap += gap;

                higherScores.Add(highScore + cumulativeGap);
            }

            higherScores.Sort();
            higherScores.Reverse();

            int decrement = Mathf.RoundToInt(Mathf.Lerp(80, 400, difficulty));

            for (int i = 0; i < numberOfBarsToSpawn; i++)
            {
                int score = highScore - decrement - Random.Range(0, decrement / 2);
                lowerScores.Add(Mathf.Max(0, score));
                decrement += Mathf.RoundToInt(decrement * 0.3f);
            }

            lowerScores.Sort();
            lowerScores.Reverse();
        }

        private LeaderBoardItem SpawnLeaderboardItems(int newRank, int lastRank, int lastScore, int playersAbovePlayer, int playersBelowPlayer, List<int> higherScores, List<int> lowerScores, int numberOfBarsToSpawn)
        {
            int higherPtr = 0;
            int lowerPtr = 0;

            int abovePlayerRank = newRank - (playersAbovePlayer + 1);

            // Above player
            for (int i = 0; i < playersAbovePlayer; i++)
            {
                LeaderBoardItem item = Instantiate(_leaderboardItem, _contentObj);
                UserData user = GetRandomUserData();

                abovePlayerRank++;
                item.SetItem(
                    abovePlayerRank,
                    higherScores[higherPtr++],
                    user.Name,
                    UserDataLoaderUtility.LoadSpriteFromResources(user.ImageFileName)
                );
            }

            // Below player
            for (int i = 0; i < playersBelowPlayer + 5; i++)
            {
                LeaderBoardItem item = Instantiate(_leaderboardItem, _contentObj);
                UserData user = GetRandomUserData();

                item.SetItem(
                    newRank,
                    lowerScores[lowerPtr++],
                    user.Name,
                    UserDataLoaderUtility.LoadSpriteFromResources(user.ImageFileName)
                );
            }

            // Player item
            LeaderBoardItem spawnedPlayerItem = Instantiate(_playerLeaderboardItem, _contentObj);
            spawnedPlayerItem.transform.SetSiblingIndex(
                _contentObj.childCount - 1 - playersBelowPlayer
            );
            spawnedPlayerItem.SetItem(lastRank, lastScore, "You");

            float startNormalizedPos = 2.25f / (numberOfBarsToSpawn + 1f);
            LeanTween.delayedCall(0.01f, () =>
            {
                _leaderboardScrollRect.verticalNormalizedPosition = startNormalizedPos;
            });

            return spawnedPlayerItem;
        }
        private bool isAnimationComplate = true;
        private void PlayLeaderboardAnimation(LeaderBoardItem playerItem, int startRank, int endRank, int startScore, int endScore, int playersAbovePlayer)
        {
            int numberOfBarsToSpawn = 0;
            float animationDelay = 0;

            float startNormalizedPos = /* 3.25f / (numberOfBarsToSpawn + 1f) */0.99f;

            int startIndex = /* playerItem.transform.GetSiblingIndex() */3;
            int endIndex = Mathf.Min(playersAbovePlayer, 3);

            int currIdx = 0;

            leaderboardTweenId = LeanTween.delayedCall(0, () =>
            {
                leaderboardTweenId = LeanTween
                    .value(gameObject, 0, 1f, 2f)
                    .setEaseInOutQuart()
                    .setOnUpdate(t =>
                    {


                        int newIdx = Mathf.RoundToInt(
                            ExtensionMethods.Map(t, 0, 1, startIndex, endIndex)
                        );

                        if (newIdx != currIdx)
                        {
                            currIdx = newIdx;
                            if (playerItem == null)
                            {
                                Debug.Log("playeritemisnull---");
                            }
                            playerItem.transform.SetSiblingIndex(currIdx);
                        }

                        playerItem.SetItem(
                            Mathf.RoundToInt(Mathf.Lerp(startRank, endRank, t)),
                            Mathf.RoundToInt(Mathf.Lerp(startScore, endScore, t)),
                            "You"
                        );
                        UpdateRank(endRank, playersAbovePlayer);
                        _leaderboardScrollRect.verticalNormalizedPosition =
                            ExtensionMethods.Map(t, 0, 1, startNormalizedPos, 1);

                    })
                    .setOnComplete(() =>
                    {
                        _close_Btn.gameObject.SetActive(true);
                        playerItem.SetItem(endRank, endScore, "You");
                        isAnimationComplate = true;

                        _isViewPanelVisible = true;
                    }).id;
            }).id;
        }
        private void UpdateRank(int endRank, int playersAbovePlayer)
        {
            int count = _contentObj.childCount;
            int startRank = endRank + 1;
            int startIndex = 4;

            if (playersAbovePlayer == 0)
            {
                startIndex = 1;
            }
            else if (playersAbovePlayer == 1)
            {
                startIndex = 2;
            }
            else if (playersAbovePlayer == 2)
            {
                startIndex = 3;
            }

            for (int i = startIndex; i < count; i++)
            {
                var item = _contentObj.GetChild(i).GetComponent<LeaderBoardItem>();
                item.SetRank(startRank);
                startRank++;
            }
        }

        private UserData GetRandomUserData()
        {
            return UserDataLoaderUtility.GetRandomUser();
        }
        private int CalculateRank(int score)
        {
            int rank = 0;

            rank = (int)ExtensionMethods.Map(score, _minScore, _maxScore, _totalPlayerCount + 1, 1);
            // Clamp the rank to be between 1 and _totalPlayerCount + 1
            rank = Mathf.Clamp(rank, 1, _totalPlayerCount + 1);
            return rank;
        }
        private int GetRankFormRange(int score)
        {
            int rank = 0;

            foreach (ScoreCategory category in _scoreCategoryList)
            {
                if (score >= category.MinScore)
                {

                    float scoreRange = (float)category.MaxScore - category.MinScore;
                    float scoreOffset = (float)score - category.MinScore;

                    float normalizedScore = scoreOffset / scoreRange;

                    normalizedScore = Mathf.Clamp01(normalizedScore);

                    rank = (int)Mathf.Lerp(category.MaxRank, category.MinRank, normalizedScore);

                    break; // Stop iteration once the correct category is found
                }
            }
            if (rank == 0 && _scoreCategoryList.Count > 0)
            {
                rank = _scoreCategoryList[_scoreCategoryList.Count - 1].MaxRank;
            }

            return rank;
        }


        public void ClearLeaderBoard()
        {
            for (int i = _contentObj.childCount - 1; i >= 0; i--)
            {
                Destroy(_contentObj.GetChild(i).gameObject);
            }
        }
        public void SetTopThreeRank()
        {
            // --- Step 1: Populate UI with Real Data or determine how many fake slots are needed ---
            int firstRank = _contentObj.GetChild(0).GetComponent<LeaderBoardItem>().Rank;

            int numberOfFakePlayersNeeded = 3;
            if (firstRank == 1)
            {
                numberOfFakePlayersNeeded = 0;
            }
            else if (firstRank == 2)
            {
                numberOfFakePlayersNeeded = 1;
            }
            else if (firstRank == 3)
            {
                numberOfFakePlayersNeeded = 2;
            }

            // --- Step 2: Generate Fake Data if required ---

            if (numberOfFakePlayersNeeded > 0)
            {
                List<int> topScores = new List<int>();
                for (int i = 0; i < numberOfFakePlayersNeeded; i++)
                {
                    int score = Random.Range(_scoreCategoryList[0].MinScore, _scoreCategoryList[0].MaxScore);
                    topScores.Add(score);
                }

                topScores.Sort();
                topScores.Reverse();

                for (int i = 0; i < numberOfFakePlayersNeeded; i++)
                {
                    UserData userData = GetRandomUserData();
                    string name = userData.Name;
                    Sprite sprite = UserDataLoaderUtility.LoadSpriteFromResources(userData.ImageFileName);
                    _topThreePlayerList[i].SetItem(i + 1, topScores[i], name, sprite);
                }
            }

            // --- Step 3: Populate the remaining slots with real data from _contentObj ---

            int currentRealItemIndex = 0;
            for (int i = numberOfFakePlayersNeeded; i < 3; i++)
            {
                if (currentRealItemIndex < _contentObj.childCount)
                {
                    LeaderBoardItem item = _contentObj.GetChild(currentRealItemIndex).GetComponent<LeaderBoardItem>();
                    if (item != null)
                    {
                        _topThreePlayerList[i].SetItem(item.Rank, item.Score, item.Name, item.Sprite);
                    }
                    currentRealItemIndex++;
                }
            }
            for (int i = numberOfFakePlayersNeeded; i < 3; i++)
            {
                if (i < _contentObj.childCount)
                {
                    // Debug.Log("Destroed" + _contentObj.GetChild(i).transform.GetInstanceID());
                    Destroy(_contentObj.GetChild(i).gameObject);
                }
            }
        }

        public void StartAutoScoreUpdate()
        {
            // if (_scoreUpdateRoutine != null)
            //     StopCoroutine(_scoreUpdateRoutine);
            _scoreUpdateRoutine = StartCoroutine(AutoScoreUpdateRoutine());
        }
        private IEnumerator AutoScoreUpdateRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(_activeDuration);

                LastScore = Score;
                Score += _scoreIncreaseValue;

                SetCurrentScore(Score);
                ShowLeaderBoard(Score, LastScore);
                // Debug.Log("highscore : " + Score);
            }
        }

        public void OnCloseBtnClick()
        {
            if (!_isViewPanelVisible)
            {
                ShowViewPanel();
            }
        }


        public void IncreaseRank(int newScore, int lastScore)
        {
            ShowViewPanel();
            ShowLeaderBoard(newScore, lastScore);
            if (_hideViewPanelCoroutine != null)
                StopCoroutine(_hideViewPanelCoroutine);
            _hideViewPanelCoroutine = StartCoroutine(HideLeadeBoardPanel());
        }
        private IEnumerator HideLeadeBoardPanel()
        {
            yield return new WaitForSeconds(_activeDuration);
            HideViewPanel();
            _hideViewPanelCoroutine = null;
        }
        private void SetVisiblePlayerItemData()
        {
            _visiblePlayerItem.SetScroe(OpsDataManager.HighScore);
            _visiblePlayerItem.SetRank(GetRankFormRange(OpsDataManager.HighScore));
        }

        #region Unity Callback
        void OnDestroy()
        {
            StopAllCoroutines();
        }
        void Update()
        {
            if (_isViewPanelVisible)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    HideViewPanel();
                }
            }
        }
        #endregion
        private void HideViewPanel()
        {
            _close_Btn.transform.LeanRotateZ(0, 0.25f);
            viewPanel.transform.LeanScaleY(0, 0.3f).setOnComplete(() =>
            {
                _isViewPanelVisible = false; ShowPlayerActiveItem();
            });
        }
        private void ShowViewPanel()
        {
            _close_Btn.transform.LeanRotateZ(180, 0.25f);
            _isViewPanelVisible = true;
            HidePlayerActiveItem(() =>
            {
                viewPanel.transform.LeanScaleY(1, 0.3f);
                spawnedPlayerItem.SetScroe(OpsDataManager.HighScore);
            });
            if (_hideViewPanelCoroutine != null)
                StopCoroutine(_hideViewPanelCoroutine);
            _hideViewPanelCoroutine = StartCoroutine(HideLeadeBoardPanel());
        }

        private void HidePlayerActiveItem(Action onComplate)
        {
            _visiblePlayerItem.transform.LeanMoveLocalY(_startPos, 0.25f).setOnComplete(() =>
            {
                _visiblePlayerItem.gameObject.SetActive(false);
                onComplate?.Invoke();
            });
        }
        private void ShowPlayerActiveItem()
        {
            _visiblePlayerItem.gameObject.SetActive(true);
            _visiblePlayerItem.transform.LeanMoveLocalY(_endPos, 0.5f);
        }
    }
}
