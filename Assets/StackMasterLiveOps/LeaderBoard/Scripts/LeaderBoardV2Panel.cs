using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using FunCell.GridMerge.System;
using FunCell.GridMerge.GamePlay;

namespace FunCell.GridMerge.UI
{
    public class LeaderBoardV2Panel : Panel
    {
        [SerializeField] private int _totalPlayerCount;
        [SerializeField] private int _minScore = 50;
        [SerializeField] private int _maxScore = 1400;
        [SerializeField] private Button _close_Btn;
        int leaderboardTweenId;
        [SerializeField] private TMP_Text _score_Label;
        [SerializeField] private LeaderBoardItem _leaderboardItem;
        [SerializeField] private LeaderBoardItem _playerLeaderboardItem;
        [SerializeField] private ScrollRect _leaderboardScrollRect;
        [SerializeField] private Transform _contentObj;
        private string[] _leaderboardNames;

        [SerializeField] private List<ScoreCategory> _scoreCategoryList;


        [SerializeField] private List<LeaderBoardItem> _topThreePlayerList;
        public int Score, LastScore;

        public override void Open()
        {
            base.Open();
            SetCurrentScore(OpsDataManager.Score);
            ShowLeaderBoard(OpsDataManager.HighScore);
            _close_Btn.onClick.AddListener(OnCloseBtnClick);
            _close_Btn.gameObject.SetActive(false);
            _close_Btn.interactable = false;
        }
        public override void Close()
        {
            base.Close();
            _close_Btn.onClick.RemoveListener(OnCloseBtnClick);
        }
        private void SetCurrentScore(int score)
        {
            _score_Label.text = score.ToString();
        }
        [ContextMenu("ShowLeaderBoard")]
        public void ShowBoard()
        {

            ShowLeaderBoard(Score, LastScore);
        }
        public void ShowLeaderBoard(int highScore = 1, int lastGameScore = 0)
        {
            ClearLeaderBoard();
            int newRank = GetRankFormRange(highScore);
            int lastRank = GetRankFormRange(lastGameScore);
            InitializeLeaderBoard(highScore, lastGameScore, newRank, lastRank);
        }
        public void InitializeLeaderBoard(int highScore, int lastScore, int newRank, int lastRank, float animationDelay = 1f)
        {
            int playersAbovePlayer = 3;
            int playersBelowPlayer = 4;
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

            int rankup = 0;
            rankup = lastRank - newRank;

            if (rankup < 0)
                rankup = 0;


            int numberOfBarsToSpawn = 5 + playersAbovePlayer + playersBelowPlayer;

            float increments = _maxScore / _totalPlayerCount;


            List<int> higherScores = new List<int>(playersAbovePlayer); // these are the scores of players above the leaderboard
            List<int> lowerScores = new List<int>(rankup + playersBelowPlayer); //these are the scores of the players which will be below the player in the leaderboard.

            if (playersAbovePlayer > 0)
            {
                for (int i = 0; i < playersAbovePlayer; i++)
                {
                    higherScores.Add((int)Random.Range(highScore + 1, highScore + increments * 2));
                }
                higherScores.Sort();
                higherScores.Reverse();
            }
            int lowerPlayersCount = rankup + playersBelowPlayer;
            if (playersBelowPlayer > 0)
            {
                for (int i = 0; i < numberOfBarsToSpawn; i++)
                {
                    int tempScore = Random.Range((int)(highScore - (increments * 3)), highScore);
                    tempScore = Mathf.Clamp(tempScore, 0, highScore);
                    Debug.Log($"increments:{increments}, increments*2:{increments * 2},(highScore - increments * 2):{(highScore - increments * 2)} ");
                    lowerScores.Add(tempScore);
                }
                lowerScores.Sort();
                lowerScores.Reverse();
            }


            Debug.Log("Last Score " + lastScore + " lowerPlayerCount " + lowerPlayersCount);
            int higherScoresPtr = 0;
            int lowerScoresPtr = 0;

            int abovePlyerRank = newRank - (playersAbovePlayer + 1);
            for (int i = 0; i < playersAbovePlayer; i++)
            {
                LeaderBoardItem item = Instantiate(_leaderboardItem, _contentObj);
                int score = 0;
                UserData userData = GetRandomUserData();
                string name = userData.Name;
                Sprite sprite = UserDataLoaderUtility.LoadSpriteFromResources(userData.ImageFileName);

                score = higherScores[higherScoresPtr];
                higherScoresPtr++;
                abovePlyerRank++;
                item.SetItem(abovePlyerRank, score, name, sprite);
            }
            if (playersBelowPlayer != 0)
            {
                for (int i = 0; i < playersBelowPlayer + 5; i++)
                {

                    LeaderBoardItem item = Instantiate(_leaderboardItem, _contentObj);
                    int score = 0;
                    int rank = newRank;
                    UserData userData = GetRandomUserData();
                    string name = userData.Name;
                    Sprite sprite = UserDataLoaderUtility.LoadSpriteFromResources(userData.ImageFileName);

                    score = lowerScores[lowerScoresPtr];
                    lowerScoresPtr++;

                    item.SetItem(rank, score, name, sprite);
                }
            }


            LeaderBoardItem playerItem = Instantiate(_playerLeaderboardItem, _contentObj);
            playerItem.transform.SetSiblingIndex(_contentObj.childCount - 1 - playersBelowPlayer); // third last
            Debug.Log("SetSiblingIndex " + (_contentObj.childCount - 1 - playersBelowPlayer));
            playerItem.SetItem(lastRank, lastScore, "You");
            float startNormalizedPos = (2.25f / (float)(numberOfBarsToSpawn + 1));

            print(startNormalizedPos);
            LeanTween.delayedCall(0.01f, () =>
            {
                _leaderboardScrollRect.verticalNormalizedPosition = startNormalizedPos;
            });

            int startIndex = playerItem.transform.GetSiblingIndex();
            int endIndex = 2;
            if (playersAbovePlayer == 0)
            {
                endIndex = 0;
            }
            else if (playersAbovePlayer == 1)
            {
                endIndex = 1;
            }
            else if (playersAbovePlayer == 2)
            {
                endIndex = 2;
            }

            int currIdx = startIndex;
            int tempNewIdx;
            int startRank = lastRank;
            int endRank = newRank;
            int startScore = lastScore;
            int endScore = highScore;
            Debug.Log($"startRank: {startRank}, endRank: {endRank},startScore : {startScore},endScore: {endScore}");
            Debug.Log($"currIndex: {currIdx}, endIndex: {endIndex}");

            //ANIMATION

            leaderboardTweenId = LeanTween.delayedCall(animationDelay, () =>
            {
                leaderboardTweenId = LeanTween.value(gameObject, 0, 1f, 2f).setEaseInOutQuart().setOnUpdate((float t) =>
                {
                    _leaderboardScrollRect.verticalNormalizedPosition = ExtensionMethods.Map(t, 0, 1, startNormalizedPos, 1);
                    tempNewIdx = Mathf.RoundToInt(ExtensionMethods.Map(t, 0, 1, startIndex, endIndex));
                    if (tempNewIdx != currIdx)
                    {
                        currIdx = tempNewIdx;
                        playerItem.transform.SetSiblingIndex(currIdx);
                    }

                    playerItem.SetItem(Mathf.RoundToInt(Mathf.Lerp(startRank, endRank, t)), Mathf.RoundToInt(Mathf.Lerp(startScore, endScore, t)), "You");

                }).setOnComplete(() =>
                {
                    float t = 1;
                    _leaderboardScrollRect.verticalNormalizedPosition = ExtensionMethods.Map(t, 0, 1, startNormalizedPos, 1);
                    tempNewIdx = Mathf.RoundToInt(ExtensionMethods.Map(t, 0, 1, startIndex, endIndex));
                    if (tempNewIdx != currIdx)
                    {
                        currIdx = tempNewIdx;
                        playerItem.transform.SetSiblingIndex(currIdx);
                    }

                    playerItem.SetItem(endRank, endScore, "You");
                    UpdateRank(endRank, playersAbovePlayer);

                    OnLeaderBoardAnimationComplate();
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

            // Iterate through the categories to find the matching one
            foreach (ScoreCategory category in _scoreCategoryList)
            {
                // Assuming categories are sorted by MinScore in descending order, 
                // or that MinScore defines the lower bound of the range.
                if (score >= category.MinScore)
                {
                    // --- 1. Calculate the Normalized Score (t) ---
                    // This is the percentage (0.0 to 1.0) of how far 'score' is 
                    // between category.MinScore and category.MaxScore.

                    float scoreRange = (float)category.MaxScore - category.MinScore;
                    float scoreOffset = (float)score - category.MinScore;

                    float normalizedScore = scoreOffset / scoreRange;

                    // Clamp the normalized score between 0 and 1, just in case 
                    // 'score' slightly exceeds MaxScore or falls below MinScore.
                    normalizedScore = Mathf.Clamp01(normalizedScore);

                    // --- 2. Calculate the Rank using Lerp ---
                    // Lerp(MaxRank, MinRank, t):
                    // - If t = 0 (MinScore), result is MaxRank (worst rank in category).
                    // - If t = 1 (MaxScore), result is MinRank (best rank in category).
                    rank = (int)Mathf.Lerp(category.MaxRank, category.MinRank, normalizedScore);

                    // Add 1 to the rank because integer ranks are usually 1-indexed (e.g., Rank 1, Rank 2)
                    // If MaxRank/MinRank are already 1-indexed, you might need to adjust this.
                    // For typical ranking systems, you might not need this +1. 
                    // Let's stick to the direct Lerp result for now, assuming MinRank/MaxRank are inclusive.

                    Debug.Log($"Score: {score}, Category Min/Max: {category.MinScore}/{category.MaxScore}, Normalized: {normalizedScore:F3}, Calculated Rank: {rank}");

                    break; // Stop iteration once the correct category is found
                }
            }

            // Safety check: if no category was found (e.g., score is too low), return a default rank.
            if (rank == 0 && _scoreCategoryList.Count > 0)
            {
                // Assuming the first category in the list is the "lowest" category
                // If the score is less than the lowest category's MinScore, 
                // return the worst possible rank.
                rank = _scoreCategoryList[_scoreCategoryList.Count - 1].MaxRank;
            }

            return rank;
        }

        private void OnLeaderBoardAnimationComplate()
        {
            SetTopThreeRank();

            _close_Btn.transform.localScale = Vector3.zero;
            _close_Btn.gameObject.SetActive(true);
            _close_Btn.transform.LeanScale(Vector3.one, 0.15f).setOnComplete(() => _close_Btn.interactable = true);
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
                    Debug.Log("Destroed" + _contentObj.GetChild(i).transform.GetInstanceID());
                    Destroy(_contentObj.GetChild(i).gameObject);
                }
            }
        }
        public void OnCloseBtnClick()
        {
            OpsSoundManager.Instance.PlayBtnClickSound();
            //PanelManager.Instance.Open<LiveOpsPopup>();
            OpsGameManager.Instance.NewGame();
        }
    }
}
