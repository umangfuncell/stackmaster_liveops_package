using UnityEngine;
using FunCell.GridMerge.UI;

namespace FunCell.GridMerge.GamePlay
{
    public class NodeAndItemSelector : BaseNodeSelector
    {
        [SerializeField] private MergeItem _selectedItem;
        [SerializeField] private Camera _camera;
        private int _itemLayerIndex;
        private int _nodeLayerIndex;
        private bool _isTouching;
        public override void Init()
        {
            _canSelect = false;

            _itemLayerIndex = LayerMask.NameToLayer("Item");
            _nodeLayerIndex = LayerMask.NameToLayer("MergeGirdNode");
            _nodeMask = (1 << _itemLayerIndex) | (1 << _nodeLayerIndex);

            OpsGameManager.OnGameStateChange += OnGameStateChange;
        }

        private void OnDestroy()
        {
            OpsGameManager.OnGameStateChange -= OnGameStateChange;
        }

        private void Update()
        {
            if (!_canSelect || OpsGameManager.Instance.GameState == GameState.Pause) return;


#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            MobileSelection();
#else
            EditorSelection();
#endif
        }

        #region Selection Core

        private void SelectItem(Transform tran)
        {
            _selectedItem = tran.parent.GetComponent<MergeItem>();
            _selectedItem.transform.LeanScale(Vector3.one * 0.8f, 0.2f);
            _selectedItem.ChangeDrgeTo(false);
        }

        private void ResetItem()
        {
            if (_selectedItem == null) return;

            if (_selectedNode != null &&
                _selectedNode.IsEmpty &&
                _selectedNode.NodeState != NodeState.Disable)
            {
                FireNodeSelect(_selectedNode, PlaceMentType.Drag);
                _selectedNode.UnHighLight();
                _selectedItem = null;
            }
            else
            {
                _selectedItem.transform
                    .LeanMoveLocal(Vector3.zero, 0.2f)
                    .setOnComplete(() =>
                    {
                        _selectedItem.ChangeDrgeTo(true);
                        _selectedItem = null;
                    });

                _selectedItem.transform.LeanScale(Vector3.one * 0.92f, 0.1f);
            }
        }

        #endregion

        #region Mobile Input (Old System)

        private void MobileSelection()
        {
            // if (Input.touchCount == 0 || OpsGameManager.Instance.istouch == false)
            // {
            //     if (_isTouching)
            //     {
            //         HandleRelease();
            //         _isTouching = false;
            //     }
            //     return;
            // }

            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = touch.position;

            if (_selectedItem != null)
            {
                MoveSelectedItem(touchPos);
            }

            Ray ray = _camera.ScreenPointToRay(touchPos);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _nodeMask))
            {
                HandleRayHit(hit, touch.phase == TouchPhase.Began);
            }
            else
            {
                ClearNode();
            }

            if (touch.phase == TouchPhase.Began)
                _isTouching = true;

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                HandleRelease();
                _isTouching = false;
            }
        }

        #endregion

        #region Editor Input (Old System)

        private void EditorSelection()
        {
            // if (OpsGameManager.Instance.istouch == false)
            // {
            //     return;
            // }
            Vector2 mousePos = Input.mousePosition;

            if (_selectedItem != null)
            {
                MoveSelectedItem(mousePos);
            }

            Ray ray = _camera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _nodeMask))
            {
                HandleRayHit(hit, Input.GetMouseButtonDown(0));
            }
            else
            {
                ClearNode();
            }

            if (Input.GetMouseButtonUp(0))
            {
                HandleRelease();
            }
        }

        #endregion

        #region Shared Logic

        private void HandleRayHit(RaycastHit hit, bool pressedThisFrame)
        {
            if (hit.transform.gameObject.layer == _nodeLayerIndex) // Node
            {
                MergeGridNode node = hit.transform.GetComponent<MergeGridNode>();

                if (_selectedNode != null && _selectedNode != node)
                    _selectedNode.UnHighLight();

                _selectedNode = node;

                if (_selectedNode != null && _selectedNode.IsEmpty &&
                    _selectedNode.NodeState != NodeState.Disable)
                {
                    _selectedNode.HighLight();

                    if (pressedThisFrame && _selectedItem == null)
                    {
                        FireNodeSelect(_selectedNode, PlaceMentType.Tap);
                        _selectedNode.UnHighLight();
                    }
                }
            }
            else if (hit.transform.gameObject.layer == _itemLayerIndex) // Item
            {
                if (pressedThisFrame && _selectedItem == null)
                {
                    SelectItem(hit.transform);
                }
            }
        }

        private void HandleRelease()
        {
            if (_selectedNode != null)
            {
                if (_selectedNode.IsEmpty && _selectedNode.NodeState != NodeState.Disable)
                {
                    FireNodeSelect(_selectedNode,
                        _selectedItem == null ? PlaceMentType.Tap : PlaceMentType.Drag);
                }

                _selectedNode.UnHighLight();
                _selectedNode = null;
            }

            ResetItem();
        }

        private void ClearNode()
        {
            if (_selectedNode != null)
            {
                _selectedNode.UnHighLight();
                _selectedNode = null;
            }
        }

        private void MoveSelectedItem(Vector2 screenPos)
        {
            Vector3 pos = screenPos;
            pos.z = _camera.nearClipPlane + 5f;
            _selectedItem.transform.position = _camera.ScreenToWorldPoint(pos);
        }

        #endregion

        private void OnGameStateChange(GameState gameState)
        {
            _canSelect = gameState == GameState.Resume;
        }
    }
}
