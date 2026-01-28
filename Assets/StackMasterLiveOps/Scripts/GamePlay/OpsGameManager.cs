using UnityEngine;
using System;
using FunCell.GridMerge.UI;
using UnityEngine.SceneManagement;
using FunCell.GridMerge.System;
namespace FunCell.GridMerge.GamePlay
{
    [DefaultExecutionOrder(-1)]
    public class OpsGameManager : MonoBehaviour
    {
        public static OpsGameManager Instance;

        [SerializeField] public MergeGrid _grid;
        [SerializeField] public ItemSpawner _spawner;
        [SerializeField] private PanelManager _panelManager;
        [SerializeField] private BaseNodeSelector _nodeSelector;
        [SerializeField] private MergeItem _mergeItem;
        [SerializeField] private Tutorial _tutorial;
        [SerializeField] private OpsAdManager _opsAdManager;
        [SerializeField] private OpsSoundManager _opsSoundManager;
        [SerializeField] private OpsAnalyticsManager _opsAnalyticsManager;
        [SerializeField] private OpsHapticManger _opsHapticManger;
        [SerializeField] private OpsRewardManger _rewardManger;
        public OpsRewardManger RewardManger => _rewardManger;

        private GameState _state;
        public GameState GameState => _state;
        public static OpsDataManager OpsDataManager;

        [SerializeField] private bool _canPlayEntryAnimation;
        public bool CanPlayEntryAnimation => _canPlayEntryAnimation;
        [SerializeField] private bool _canPlayExitAnimation;
        public bool CanPlayExitAnimation => _canPlayExitAnimation;
        public GamePanel gamePanel;
        public AudioSource Music;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            OpsDataManager = new OpsDataManager();
            OpsDataManager.Init();
            _opsAnalyticsManager.InIt();
            _opsSoundManager.Init();
            _opsAdManager.InIt();
            _opsHapticManger.Init();
            _rewardManger.InIt();
            _spawner.Init(_mergeItem);
            _grid.Init(_spawner, OpsDataManager, _nodeSelector, _mergeItem);
            _nodeSelector.Init();
            _panelManager.Init();

            if (OpsDataManager.CanPlayTutorial())
                _tutorial.Init(OpsDataManager);

        }

        public void OnOffMusic(bool isOnOffMusic)
        {
            if (isOnOffMusic)
            {

            }
        }

        public static event Action<GameState> OnGameStateChange;
        public void ChangeStateTo(GameState state)
        {
            _state = state;
            switch (_state)
            {
                case GameState.Start:
                    GameStart();
                    break;
                case GameState.Pause:
                    GamePause();
                    break;
                case GameState.Resume:
                    GameResume();
                    break;
                case GameState.Win:
                    GameWin();
                    break;
                case GameState.Over:
                    GameOver();
                    break;
            }
            OnGameStateChange?.Invoke(_state);
        }
        public void ChangeBlock(MergeItemLevel level, int cost)
        {
            if (OpsDataManager.CheckAndUseJoker(cost))
                _spawner.ChangeFirstItem(level);
        }
        public void RVChangeBlock(MergeItemLevel level)
        {
            _spawner.ChangeFirstItem(level);
        }

        public void NewGame()
        {
            if (OpsDataManager.CanPlayTutorial())
            {
                _tutorial.Init(OpsDataManager);
            }
            else
            {
                _tutorial.gameObject.SetActive(false);
                _grid.StartNewGame(_canPlayEntryAnimation);
                _spawner.StartNewGame(_canPlayEntryAnimation);
            }
            OpsDataManager.ResetScore();
            ChangeStateTo(GameState.Start);
        }
        public void LoadGame()
        {
            if (OpsDataManager.CanContinueGame())
            {
                GameDataClass gameDataClass = OpsDataManager.Loadgame();
                _grid.LoadOldGrid(gameDataClass, _canPlayEntryAnimation);
                _spawner.LoadOldItems(gameDataClass.spawnerItems, _canPlayEntryAnimation);
                ChangeStateTo(GameState.Start);
            }
            // else
            // {
            //     Debug.Log("Game Not Found");
            // }
        }
        public void SaveGame()
        {
            GameDataClass gameDataClass = new GameDataClass();

            gameDataClass.gridItems = _grid.GetItemsOfGrid();
            gameDataClass.spawnerItems = _spawner.GetItems();
            OpsDataManager.SaveGame(gameDataClass);
        }
        public void UnLoadGame()
        {
            _grid.OnGridChange -= SaveGame;
            _spawner.OnSpawnerChanged -= SaveGame;
            _spawner.UnLoad(_canPlayExitAnimation);
            _grid.UnLoadGrid(() =>
            {
                _panelManager.Open<MenuPanel>();
                ChangeStateTo(GameState.None);
            }, _canPlayExitAnimation);
        }

        private void GameStart()
        {
            _grid.OnGridChange += SaveGame;
            _spawner.OnSpawnerChanged += SaveGame;
        }
        private void GameOver()
        {
            _grid.OnGridChange -= SaveGame;
            _spawner.OnSpawnerChanged -= SaveGame;
            _spawner.UnLoad();
            _grid.UnLoadGrid(() =>
            {
                _panelManager.Open<GameOverPanel>();
            });
            OpsDataManager.ClearGame();
        }
        private void GamePause()
        {
        }
        private void GameResume()
        {
        }
        private void GameWin()
        {
            _grid.OnGridChange -= SaveGame;
            _spawner.OnSpawnerChanged -= SaveGame;
        }
        public MergeGridNode GetEmptyNode()
        {
            return _grid.GetEmptyNode();
        }
        public void UnlockNewItem(MergeItemLevel mergeItemLevel)
        {
            OpsDataManager.UnLockNewItemLevel(mergeItemLevel);
            // GameAnalyticsController.Miscellaneous.NewDesignEvent($"item_merge:item_{mergeItemLevel}");
            SaveGame();
        }
        [ContextMenu("DeleteKeyTutorialKey")]
        public void DeleteKey()
        {
            PlayerPrefs.DeleteKey("Tutorial");
        }
        [ContextMenu("GameOver")]
        public void RiseGameOver()
        {
            ChangeStateTo(GameState.Over);
        }
    }
    public enum GameState
    {
        None,
        Start,
        Pause,
        Resume,
        Win,
        Over
    }
}

