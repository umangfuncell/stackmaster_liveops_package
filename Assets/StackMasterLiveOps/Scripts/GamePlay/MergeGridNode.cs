using UnityEngine;
using UnityEngine.UI;
using System;
namespace FunCell.GridMerge.GamePlay
{
    public class MergeGridNode : MonoBehaviour
    {
        [SerializeField] private int _row;
        [SerializeField] private int _column;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private MergeItem _item;
        [SerializeField] private NodeState _initialState;
        [SerializeField] private GameObject _highLightObject;
        [SerializeField] private GameObject _rvIconObj;
        [SerializeField] private SpriteRenderer _tileSpriteRenderer;
        [SerializeField] private Transform _interactableTile;
        [SerializeField] private Transform _unInteractableTile;
        [SerializeField] private Collider _collider;
        private MergeGrid _grid;

        //private JaggedMergeGrid _grid;
        public int Row => _row;
        public int Column => _column;
        public MergeItem Item => _item;
        public bool IsEmpty { get { return _item == null; } }

        public NodeState NodeState { get; private set; }
        //public void Init(MergeGrid grid,int row,int column)
        //{
        //    _grid = grid;
        //    _row = row;
        //    _column = column;
        //}
        public void Init(MergeGrid grid, int row, int column)
        {
            _grid = grid;
            _row = row;
            _column = column;
            NodeState = _initialState;
            switch (NodeState)
            {
                case NodeState.Available:
                    _rvIconObj.gameObject.SetActive(false);
                    break;
                case NodeState.Disable:
                    gameObject.SetActive(false);
                    break;
                case NodeState.RV:
                    _rvIconObj.gameObject.SetActive(true);
                    gameObject.SetActive(false);
                    break;
            }
        }
        public void LoadState(NodeState state)
        {
            Debug.Log("Loading state success fully");
            NodeState = state;
            switch (NodeState)
            {
                case NodeState.Available:
                    _rvIconObj.gameObject.SetActive(false);
                    gameObject.SetActive(true);
                    break;
                case NodeState.Disable:
                    gameObject.SetActive(false);
                    break;
                case NodeState.RV:
                    _rvIconObj.gameObject.SetActive(true);
                    gameObject.SetActive(false);
                    break;
            }
        }

        public void HighLight()
        {
            if (IsEmpty)
                _highLightObject.SetActive(true);
        }
        public void UnHighLight()
        {
            _highLightObject.SetActive(false);
        }

        public void TryToUnlock(Action onComplate, int tilenumber)
        {
            // AdsManager_LevelPlay.instance.ShowRewardAd(() =>
            // {
            //     NodeState = NodeState.Available;
            //     _rvIconObj.gameObject.SetActive(false);
            //     onComplate?.Invoke();
            // }, $"add_tile_{tilenumber}");
            // Debug.LogError("TryToUnlock");
            OpsAdManager.Instance.ShowRewardedAd(() => UnlockSuccess(onComplate), UnlockFail, "liveops:tile_unlock_" + tilenumber.ToString());
        }
        public void UnlockSuccess(Action onComplate)
        {
            NodeState = NodeState.Available;
            _rvIconObj.gameObject.SetActive(false);
            onComplate?.Invoke();
        }
        public void UnlockFail()
        {
            NodeState = NodeState.RV;
            _rvIconObj.gameObject.SetActive(true);
            OpsGameManager.Instance._grid._seletor.CanSelect(true);
            OpsGameManager.Instance.SaveGame();
        }
        public void PlaceItemOnGridNode(MergeItem item, PlaceMentType placeMentType, Action onComplate, bool isSpawning = false)
        {
            _item = item;
            _item.transform.parent = transform;

            if (isSpawning)
            {
                _item.ChangeDrgeTo(false);
                _item.SetPositionAndRotation(_spawnPoint.position + (Vector3.up * 0.6f), transform.rotation);
                _item.PopDown(_spawnPoint.position, onComplate);
            }
            else
            {
                _item.SetRotation(_spawnPoint.rotation);
                if (placeMentType == PlaceMentType.Tap)
                    _item.GotoGridNode(_spawnPoint.position, onComplate);
                else
                    _item.GotoGridNodeLinear(_spawnPoint.position, onComplate);
            }
        }
        public void RemoveItem()
        {
            if (IsEmpty) return;
            _item.transform.parent = null;
            _item = null;
        }

        public void Reset()
        {
            NodeState = _initialState;
            switch (NodeState)
            {
                case NodeState.Available:
                    _rvIconObj.gameObject.SetActive(false);
                    break;
                case NodeState.Disable:
                    gameObject.SetActive(false);
                    break;
                case NodeState.RV:
                    _rvIconObj.gameObject.SetActive(true);
                    gameObject.SetActive(false);
                    break;
            }
        }

        public Vector3 GetSpawnPoint()
        {
            return _spawnPoint.position;
        }
        public void SetUnInteractable()
        {
            _collider.enabled = false;

            if (_tileSpriteRenderer != null)
                _tileSpriteRenderer.color = new Color(1, 1, 1, 0.5f);

            // if (_unInteractableTile != null && _interactableTile != null)
            // {
            //     _unInteractableTile.gameObject.SetActive(true);
            //     _interactableTile.gameObject.SetActive(false);
            // }
            // else
            // {
            //     Debug.LogError("_unInteractableTile or _interactableTile transform is missing");
            // }
        }
        public void SetInteractable()
        {
            _collider.enabled = true;

            if (_tileSpriteRenderer != null)
                _tileSpriteRenderer.color = new Color(1, 1, 1, 1f);

            // if (_unInteractableTile != null && _interactableTile != null)
            // {
            //     _unInteractableTile.gameObject.SetActive(false);
            //     _interactableTile.gameObject.SetActive(true);
            // }
            // else
            // {
            //     Debug.LogError("_unInteractableTile or _interactableTile transform is missing");
            // }
        }
    }
    public enum NodeState
    {
        Available,
        Disable,
        RV
    }
}
