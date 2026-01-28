using System.Collections.Generic;
using FunCell.GridMerge.GamePlay;
using FunCell.GridMerge.UI;
using UnityEngine;
using FunCell.GridMerge.System;
using UnityEngine.Video;
public class Tutorial : MonoBehaviour
{
    public static Tutorial Instance;
    [SerializeField] private MergeItem _mergeItem;
    [SerializeField] private MergeGrid _mergeGrid;

    [SerializeField] private ItemSpawner _itemSpawner;

    [SerializeField] private List<SpawnerSlot> _spawnerSlots;
    [SerializeField] private List<MergeItemLevel> _spawnerItemList;

    [SerializeField] private List<MergeGridNode> _nodeList;
    [SerializeField] private List<MergeItemLevel> _spawnItemLevelList;

    [SerializeField] private List<MergeGridNode> _allNodes;

    [SerializeField] private BaseNodeSelector _nodeSelector;

    [SerializeField] private TutorialPanel _tutorialPanel;

    [SerializeField] private Transform _ui_hand;

    [Header("Tutorial Steps")]
    [Header("One")]
    [SerializeField] private MergeGridNode _firstNode;
    [SerializeField] private Transform _stepOne_HandPoint;
    [Header("Two")]
    [SerializeField] private MergeGridNode _secondNode;
    [SerializeField] private Transform _stepTwo_HandPoint;

    [Header("Three")]
    [SerializeField] private GamePanel _gamePanel;
    [SerializeField] private Transform _stepThree_HandPoint;

    // [Header("Three And Half")]
    // [SerializeField] private JokerPanel _jokerPanel;
    // [SerializeField] private Transform _stepThreeAndHalf_HandPoint;

    [Header("Four")]
    [SerializeField] private MergeGridNode _fourNode;
    [SerializeField] private Transform _stepFour_HandPoint;

    [SerializeField] private int _tutorialStep;
    [SerializeField] private List<Transform> handPoints;
    private OpsDataManager _opsdataManager;

    public void Init(OpsDataManager dataManager)
    {
        Instance = this;
        _opsdataManager = dataManager;
        gameObject.SetActive(true);
        // SpawnTutorialItem();
    }
    public void SpawnTutorialItem()
    {
        _mergeGrid.StartTutorial(_nodeList, _spawnItemLevelList);
        _itemSpawner.StartTutorial(_spawnerItemList);
        StepOne();
    }
    public void Update()
    {
        if (_tutorialStep == 1)
        {
            if (_firstNode.Item != null)
            {
                _tutorialStep = 2;
                StepTwo();
            }
        }
        else if (_tutorialStep == 2)
        {
            if (_secondNode.Item != null)
            {
                _tutorialStep = 4;

                // StepThree();
                EndTutorial();
                 PanelManager.Instance.Open<GamePanel>();
            }
        }
        else if (_tutorialStep == 3)
        {
            if (_itemSpawner.GetFirstItem().ItemLevel == MergeItemLevel.Level2)
            {
                _tutorialStep = 4;
                StepFive();
            }
        }
        else
        {
            if (_fourNode.Item != null)
            {
                EndTutorial();
            }
        }
 
    }
    private void StepOne()
    {
        _tutorialPanel.Show();
        _tutorialPanel.StepOne();
        _ui_hand.gameObject.SetActive(true);
        // _ui_hand.transform.localScale = Vector3.zero;
        _ui_hand.transform.LeanMove(Camera.main.WorldToScreenPoint(_stepOne_HandPoint.position), 0.2f);
        _nodeSelector.CanSelect(true);
        DisableAllNodeCollider();
        _firstNode.SetInteractable();
        _tutorialStep = 1;
    }
    private void StepTwo()
    {
        _tutorialPanel.StepTwo();
        _ui_hand.transform.LeanMove(Camera.main.WorldToScreenPoint(_stepTwo_HandPoint.position), 0.2f);
        DisableAllNodeCollider();
        _secondNode.SetInteractable();
    }
    private void StepThree()
    {
        _tutorialPanel.StepThree();
       // _ui_hand.transform.eulerAngles = new Vector3(0, 180, 0);
        _ui_hand.transform.LeanMove(_stepThree_HandPoint.position, 0.2f);
        DisableAllNodeCollider();
        PanelManager.Instance.Open<GamePanel>();
        _gamePanel.StartTutorial(this);
    }
    private void StepFive()
    {
        _tutorialPanel.StepFour();
        _ui_hand.transform.LeanMove(Camera.main.WorldToScreenPoint(_stepFour_HandPoint.position), 0.2f);
        DisableAllNodeCollider();
        _fourNode.SetInteractable();
    }
    private void EndTutorial()
    {
        // Debug.LogError("EndTutorial...........");
        _opsdataManager.SaveTutotrial();
        _opsdataManager.IsOpsTutorialFinished = true;
        _ui_hand.gameObject.SetActive(false);
        foreach (MergeGridNode node in _allNodes)
        {
            node.SetInteractable();
        }
        _tutorialPanel.Hide();
        // _jokerPanel.EndTutorial();
        _mergeGrid.EndTurorial();
        gameObject.SetActive(false);
        _gamePanel.EndTutorial();
    }
    // public void JokerCLicked()
    // {
    //     _tutorialStep = 4;
    //     _ui_hand.transform.eulerAngles = new Vector3(0, 0, 0);
    //     _ui_hand.transform.LeanMove(_stepThreeAndHalf_HandPoint.position, 0.2f);
    //     _tutorialPanel.JokerClicked();
    //     // _jokerPanel.StartTutorial();

    // }
    private void DisableAllNodeCollider()
    {
        foreach (MergeGridNode node in _allNodes)
        {
            node.SetUnInteractable();
        }
    }

}
