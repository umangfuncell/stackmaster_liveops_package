using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;
using System.Collections;
using FunCell.GridMerge.UI;
using FunCell.GridMerge.System;

namespace FunCell.GridMerge.GamePlay
{
    public class MergeGrid : MonoBehaviour
    {
        [SerializeField] private int _totalRow;
        [SerializeField] private int _totalColumn;
        [SerializeField] private int _preSpawnItemCount;
        [SerializeField] private float _enterExitAnimationTime = 0.3f;
        [SerializeField] private GameObject _scoreLablePrefab;
        [SerializeField] private Transform _baseTransform;
        [SerializeField] private MergeItemLevel _maxItemLevel;
        [SerializeField] private List<MergeGridNode> _rvNodeList;
        private int _rvNodeCounter = 0;
        private MergeGridNode[,] _nodes;


        private ItemSpawner _spawner;
        private OpsDataManager _opsDataManager;
        public BaseNodeSelector _seletor;
        private MergeItem _item;
        private int _combo;
        private bool _isSpawnerRemovedItem;
        private bool _isMergeingDone;
        public event Action OnGridChange;

        public void Init(ItemSpawner spawner, OpsDataManager opsDataManager, BaseNodeSelector selector, MergeItem mergeItem)
        {
            _item = mergeItem;
            _spawner = spawner;
            _seletor = selector;
            _opsDataManager = opsDataManager;
            _nodes = new MergeGridNode[_totalRow, _totalColumn];
            transform.position = new Vector3(transform.position.x, -5, transform.position.z);
            if (_baseTransform != null)
                _baseTransform.position = new Vector3(transform.position.x, -11.08f, transform.position.z);
            int index = 0;

            for (int i = 0; i < _totalRow; i++)
            {
                for (int j = 0; j < _totalColumn; j++)
                {
                    MergeGridNode node = transform.GetChild(index).GetComponent<MergeGridNode>();
                    node.Init(this, i, j);
                    _nodes[i, j] = node;
                    index++;
                }
            }

            gameObject.SetActive(false);

            if (_baseTransform != null)
                _baseTransform.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            // NodeSelector.OnNodeSelect += OnNodeSelect;
            BaseNodeSelector.OnNodeSelect += OnNodeSelect;
        }
        private void OnDisable()
        {
            //NodeSelector.OnNodeSelect -= OnNodeSelect;
            BaseNodeSelector.OnNodeSelect -= OnNodeSelect;
        }

        public void StartNewGame(bool canAnimate = false)
        {
            gameObject.SetActive(true);

            if (_baseTransform != null)
                _baseTransform.gameObject.SetActive(true);

            if (_rvNodeList.Count > 0)
            {
                _rvNodeList[0].gameObject.SetActive(true);
            }

            if (canAnimate)
            {
                // Debug.Log("aimate and load");
                transform.LeanMoveY(0, _enterExitAnimationTime).setOnComplete(() => { CreateRandomMergeItem(); });
                if (_baseTransform != null)
                    _baseTransform.LeanMoveY(-6f, _enterExitAnimationTime);
            }
            else
            {
                transform.position = Vector3.zero;
                if (_baseTransform != null)
                    _baseTransform.position = new Vector3(0, -6, 0);
                CreateRandomMergeItem();
            }
        }
        public void LoadOldGrid(GameDataClass gameDataClass, bool canAnimate = false)
        {
            gameObject.SetActive(true);

            if (_baseTransform != null)
                _baseTransform.gameObject.SetActive(true);

            for (int i = 0; i < gameDataClass.rvNodeStatesList.Length; i++)
            {
                _rvNodeList[i].LoadState(gameDataClass.rvNodeStatesList[i]);
            }

            _rvNodeCounter = gameDataClass.rvNodeCounter;
            if (_rvNodeCounter < _rvNodeList.Count)
            {
                _rvNodeList[_rvNodeCounter].gameObject.SetActive(true);
            }


            if (canAnimate)
            {
                transform.LeanMoveY(0, _enterExitAnimationTime).setOnComplete(() => StartCoroutine(SetOldGridSequence(gameDataClass.gridItems)));
                if (_baseTransform != null)
                    _baseTransform.LeanMoveY(-6f, _enterExitAnimationTime);
            }
            else
            {
                transform.position = Vector3.zero;
                if (_baseTransform != null)
                    _baseTransform.position = new Vector3(0, -6, 0);
                SetOldGridSequence(gameDataClass.gridItems);
            }
        }
        public void UnLoadGrid(Action onComplate, bool canAnimate = false)
        {
            _seletor.CanSelect(false);
            _rvNodeCounter = 0;
            if (canAnimate)
            {
                //this loop send item down for unload aniamtion
                foreach (MergeGridNode node in _nodes)
                {
                    if (!node.IsEmpty)
                    {
                        node.Item.GoDown();
                    }
                }

                transform.LeanMoveY(-5, _enterExitAnimationTime).setOnComplete(() =>
                {
                    foreach (MergeGridNode node in _nodes)
                    {
                        if (!node.IsEmpty)
                            Destroy(node.Item.gameObject);
                        node.RemoveItem();
                        node.Reset();
                    }
                    onComplate?.Invoke();
                    gameObject.SetActive(false);
                    if (_baseTransform != null)
                        _baseTransform.gameObject.SetActive(false);
                });


                if (_baseTransform != null)
                    _baseTransform.LeanMoveY(-11.08f, _enterExitAnimationTime);
            }
            else
            {
                if (_baseTransform != null)
                    _baseTransform.position = new Vector3(transform.position.x, -11.08f, transform.position.z);
                transform.position = new Vector3(transform.position.x, -5, transform.position.z);
                foreach (MergeGridNode node in _nodes)
                {
                    if (!node.IsEmpty)
                        Destroy(node.Item.gameObject);
                    node.RemoveItem();
                    node.Reset();
                }
                onComplate?.Invoke();
                gameObject.SetActive(false);
                if (_baseTransform != null)
                    _baseTransform.gameObject.SetActive(false);
            }
        }

        public void RemoveItemsAndContinue(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int row = Random.Range(0, _totalRow);
                int col = Random.Range(0, _totalColumn);

                MergeGridNode node = _nodes[row, col];
                if (node.IsEmpty)
                {
                    i--;
                }
                else
                {
                    Destroy(node.Item.gameObject);
                    node.RemoveItem();
                }
            }
        }

        public static Vector3[] GetItemMovePath(Vector3 startPosition, Vector3 endPosition, float curve = 2)
        {
            Vector3 p1 = Vector3.Lerp(startPosition, endPosition, 0.33f);
            p1.y += curve;

            Vector3 p2 = Vector3.Lerp(startPosition, endPosition, 0.66f);
            p2.y += curve;

            return new Vector3[] { startPosition, p1, p2, endPosition };
        }
        public MergeItemLevel[,] GetItemsOfGrid()
        {
            MergeItemLevel[,] itemLevels = new MergeItemLevel[_totalRow, _totalColumn];
            for (int i = 0; i < _totalRow; i++)
            {
                for (int j = 0; j < _totalColumn; j++)
                {
                    if (_nodes[i, j].IsEmpty)
                    {
                        itemLevels[i, j] = MergeItemLevel.None;
                    }
                    else
                    {
                        itemLevels[i, j] = _nodes[i, j].Item.ItemLevel;
                    }
                }
            }
            return itemLevels;
        }
        public NodeState[] GetRvNodeStateList()
        {
            NodeState[] nodeStates = new NodeState[_rvNodeList.Count];
            for (int i = 0; i < _rvNodeList.Count; i++)
            {
                nodeStates[i] = _rvNodeList[i].NodeState;
            }
            return nodeStates;
        }
        public int GetRvNodeCounter()
        {
            return _rvNodeCounter;
        }

        private IEnumerator SetOldGridSequence(MergeItemLevel[,] oldItems)
        {
            for (int i = 0; i < _totalRow; i++)
            {
                for (int j = 0; j < _totalColumn; j++)
                {
                    if (oldItems[i, j] == MergeItemLevel.None)
                    {
                        continue;
                    }

                    MergeItem item = Instantiate(_item);
                    item.SetLevel(oldItems[i, j]);

                    bool wait = true;
                    _nodes[i, j].PlaceItemOnGridNode(item, PlaceMentType.Tap, () => wait = false, true);

                    while (wait)
                        yield return null;

                    yield return new WaitForSeconds(0.1f);
                }
            }
            _seletor.CanSelect(true);
            PanelManager.Instance.Open<GamePanel>();
        }

        private void CreateRandomMergeItem()
        {
            StartCoroutine(CreateSequence());
        }
        private IEnumerator CreateSequence()
        {
            for (int i = 0; i < _preSpawnItemCount; i++)
            {
                bool placed = false;

                for (int j = 0; j < 20; j++)
                {
                    int randomRow = Random.Range(0, _totalRow);
                    int randomCol = Random.Range(0, _totalColumn);
                    MergeGridNode node = _nodes[randomRow, randomCol];

                    if (!node.IsEmpty || node.NodeState != NodeState.Available)
                        continue;

                    for (int attempt = 0; attempt < 10; attempt++)
                    {
                        MergeItemLevel level = MergeItem.GetRandomLevel();
                        if (WouldCauseMerge(randomRow, randomCol, level))
                            continue;

                        MergeItem item = Instantiate(_item);
                        item.SetLevel(level);

                        bool wait = true;
                        node.PlaceItemOnGridNode(item, PlaceMentType.Tap, () => wait = false, true);
                        SpawnScoreLable(item.GetScore().ToString(), node);

                        UpdateScore(item.GetScore());
                        placed = true;

                        while (wait)
                            yield return null;

                        break;
                    }

                    if (placed)
                        break;
                }

                yield return new WaitForSeconds(0.2f);
            }
            _seletor.CanSelect(true);
            OnGridChange?.Invoke();
            PanelManager.Instance.Open<GamePanel>();
        }

        private bool WouldCauseMerge(int startRow, int startCol, MergeItemLevel level)
        {
            if (!_nodes[startRow, startCol].IsEmpty)
                return false;

            int rows = _nodes.GetLength(0);
            int cols = _nodes.GetLength(1);

            var visited = new bool[rows, cols];
            var q = new Queue<(int r, int c)>();

            q.Enqueue((startRow, startCol));
            visited[startRow, startCol] = true;
            int count = 0;

            while (q.Count > 0)
            {
                var (r, c) = q.Dequeue();
                count++;

                if (count >= 3)
                    return true;

                // Check 4 neighbors
                TryEnqueueNeighbor(r + 1, c, level, visited, q);
                TryEnqueueNeighbor(r - 1, c, level, visited, q);
                TryEnqueueNeighbor(r, c + 1, level, visited, q);
                TryEnqueueNeighbor(r, c - 1, level, visited, q);
            }

            return false;
        }
        private void TryEnqueueNeighbor(int r, int c, MergeItemLevel level, bool[,] visited, Queue<(int, int)> q)
        {
            if (r < 0 || c < 0 || r >= _nodes.GetLength(0) || c >= _nodes.GetLength(1))
                return;

            if (visited[r, c])
                return;

            var node = _nodes[r, c];

            if (node.IsEmpty)
                return;

            if (node.Item.ItemLevel != level)
                return;

            visited[r, c] = true;
            q.Enqueue((r, c));
        }
        private void OnNodeSelect(MergeGridNode node, PlaceMentType placeMentType)
        {
            if (node.IsEmpty)
            {
                _seletor.CanSelect(false);
                switch (node.NodeState)
                {
                    case NodeState.Available:

                        _isMergeingDone = false;
                        _isSpawnerRemovedItem = false;
                        _combo = 0;
                        MergeItem item = _spawner.GetFirstItem();
                        _spawner.RemoveFirstItem(() =>
                        {
                            _isSpawnerRemovedItem = true;
                            TryToEnableSelection();
                        });
                        UpdateScore(item.GetScore());
                        SpawnScoreLable(item.GetScore().ToString(), node);
                        node.PlaceItemOnGridNode(item, placeMentType, () => { CheckForMerge(node); });
                        if (OpsGameManager.OpsDataManager.IsOpsTutorialFinished == true)
                        {
                            OpsGameManager.Instance.gamePanel._ui_hand.gameObject.SetActive(false);
                            OpsGameManager.Instance.gamePanel.StopHintTutorialIEnumerator();
                        }

                        break;

                    case NodeState.Disable:
                        _seletor.CanSelect(true);
                        break;

                    case NodeState.RV:
                        node.TryToUnlock(() =>
                        {
                            _seletor.CanSelect(true);
                            ActiveNextRvTile();
                            OpsGameManager.Instance.SaveGame();
                        }, _rvNodeCounter + 1);
                        break;
                }
                OnGridChange?.Invoke();
            }
        }
        private void CheckForMerge(MergeGridNode startNode)
        {
            //if (startNode.Item.ItemLevel == _maxItemLevel)
            //{
            //    _isMergeingDone = true;
            //    CheckGameOver();
            //    return;
            //}

            var level = startNode.Item.ItemLevel;
            List<MergeGridNode> group = GetConnectedSameLevel(startNode.Row, startNode.Column, level);

            if (group.Count >= 3)
            {
                _combo++;
                OnGridChange?.Invoke();
                MergeItems(startNode, group);
            }
            else
            {
                _isMergeingDone = true;
                CheckGameOver();
            }
        }
        private void MergeItems(MergeGridNode startNode, List<MergeGridNode> group)
        {
            MergeItem mainItem = startNode.Item;
            CalculateMergeScore(mainItem, group.Count);

            int totalAnimations = 0;
            int completedAnimations = 0;

            foreach (MergeGridNode node in group)
            {
                if (node == startNode)
                    continue;

                MergeItem item = node.Item;
                totalAnimations++;

                item.CurveMove(GetItemMovePath(item.transform.position, mainItem.transform.position), () =>
                {
                    node.RemoveItem();
                    Destroy(item.gameObject);
                    completedAnimations++;
                    if (completedAnimations == totalAnimations)
                    {
                        SpawnScoreLable((mainItem.GetScore() * (group.Count - 1)).ToString(), startNode);
                        UpgradeMainItem(mainItem, startNode);
                    }
                });
            }
        }
        private void UpgradeMainItem(MergeItem item, MergeGridNode startNode)
        {
            item.Upgrade();
            CheckForMerge(startNode);
        }
        private List<MergeGridNode> GetConnectedSameLevel(int startRow, int startCol, MergeItemLevel level)
        {
            List<MergeGridNode> result = new List<MergeGridNode>();
            Queue<MergeGridNode> queue = new Queue<MergeGridNode>();
            HashSet<MergeGridNode> visited = new HashSet<MergeGridNode>();

            var startNode = _nodes[startRow, startCol];
            queue.Enqueue(startNode);
            visited.Add(startNode);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                result.Add(node);

                TryAddNeighbor(node.Row + 1, node.Column, level, queue, visited);
                TryAddNeighbor(node.Row - 1, node.Column, level, queue, visited);
                TryAddNeighbor(node.Row, node.Column + 1, level, queue, visited);
                TryAddNeighbor(node.Row, node.Column - 1, level, queue, visited);
            }

            return result;
        }
        private void TryAddNeighbor(int r, int c, MergeItemLevel level, Queue<MergeGridNode> queue, HashSet<MergeGridNode> visited)
        {
            if (r < 0 || c < 0 || r >= _nodes.GetLength(0) || c >= _nodes.GetLength(1))
                return;

            var node = _nodes[r, c];

            if (node.NodeState != NodeState.Available)
                return;

            // Ignore empty nodes
            if (node.IsEmpty)
                return;

            // Check same item level
            if (node.Item.ItemLevel != level)
                return;

            if (!visited.Contains(node))
            {
                visited.Add(node);
                queue.Enqueue(node);
            }
        }
        private void CheckGameOver()
        {
            for (int i = 0; i < _totalRow; i++)
            {
                for (int j = 0; j < _totalColumn; j++)
                {
                    if (_nodes[i, j].IsEmpty && _nodes[i, j].NodeState == NodeState.Available)
                    {
                        OnGridChange?.Invoke();
                        TryToEnableSelection();
                        return;
                    }
                }
            }
            _isMergeingDone = false;
            _isSpawnerRemovedItem = false;
            _seletor.CanSelect(false);
            PanelManager.Instance.Open<NoSpaceLeft>();
        }
        private void CalculateMergeScore(MergeItem mainItem, int mergeCount)
        {
            int itemScore = mainItem.GetScore();
            int score = ((mergeCount - 1) * itemScore) * _combo;
            UpdateScore(score);
        }
        private void UpdateScore(int value)
        {
            _opsDataManager.AddScore(value);
        }
        private void SpawnScoreLable(string text, MergeGridNode node)
        {
            if (_combo > 1)
            {
                text = text + "x" + _combo.ToString();
            }
            Instantiate(_scoreLablePrefab).GetComponent<WorldScoreLable>().Init(node.GetSpawnPoint(), Camera.main.transform.rotation, text);
        }
        private void TryToEnableSelection()
        {
            if (_isSpawnerRemovedItem && _isMergeingDone)
                _seletor.CanSelect(true);
        }
        private void ActiveNextRvTile()
        {
            // Debug.Log("im called" + _rvNodeCounter);
            _rvNodeCounter++;
            if (_rvNodeCounter < _rvNodeList.Count)
            {
                _rvNodeList[_rvNodeCounter].gameObject.SetActive(true);
            }
        }

        public bool IsGameOver()
        {
            for (int i = 0; i < _totalRow; i++)
            {
                for (int j = 0; j < _totalColumn; j++)
                {
                    if (_nodes[i, j].IsEmpty && _nodes[i, j].NodeState == NodeState.Available)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public MergeGridNode GetEmptyNode()
        {
            foreach (MergeGridNode node in _nodes)
            {
                if (node.IsEmpty && node.NodeState == NodeState.Available)
                {
                    return node;
                }
            }
            return null;
        }
        #region Tutorial
        public void StartTutorial(List<MergeGridNode> nodes, List<MergeItemLevel> mergeItemLevels)
        {
            gameObject.SetActive(true);
            if (_baseTransform != null)
                _baseTransform.gameObject.SetActive(true);


            transform.LeanMoveY(0, _enterExitAnimationTime).setOnComplete(() => CreateTutorialItem(nodes, mergeItemLevels));
            if (_baseTransform != null)
                _baseTransform.LeanMoveY(-6f, _enterExitAnimationTime);
        }
        private void CreateTutorialItem(List<MergeGridNode> nodes, List<MergeItemLevel> mergeItemLevels)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                MergeItem item = Instantiate(_item);
                item.Init(mergeItemLevels[i]);
                nodes[i].PlaceItemOnGridNode(item, PlaceMentType.Tap, null, true);
            }
        }
        public void EndTurorial()
        {
            if (_rvNodeList.Count > 0)
            {
                _rvNodeList[0].gameObject.SetActive(true);
            }
        }
        #endregion
    }
}


