using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using FunCell.GridMerge.UI;
using FunCell.GridMerge.System;
using FunCell.GridMerge.GamePlay;
using System.Collections;
public class StarBtnHadler : MonoBehaviour
{
    [SerializeField] private float _aniamtionDuration = 0.2f;
    [SerializeField] private RectTransform _rectTransform;

    [SerializeField] private List<JokerShopBtn> _btnList;
    [SerializeField] private List<MergeItemLevel> _itemLevelList;
    [SerializeField] private CostList _costSO;
    [SerializeField] private ItemSpriteList _itemSpriteList;

    private bool isHidden;

    public void InitJokerButtones()
    {
        // Debug.Log("start btn position " + transform.localPosition);
        int index = 0;
        foreach (JokerShopBtn jokerShopBtn in _btnList)
        {
            jokerShopBtn.Init(OpsDataManager.Joker, _costSO.list[index], _itemLevelList[index], _itemSpriteList.sprites[index]);
            index++;
        }
        //Show();
        foreach (JokerShopBtn jokerShopBtn in _btnList)
        {
            jokerShopBtn.HandleState();
        }
        OpsDataManager.OnNewItemUnlock += OnNewItemUnlock;
    }
    public void ResetJokerButtones()
    {
        foreach (JokerShopBtn jokerShopBtn in _btnList)
        {
            jokerShopBtn.Reset();
        }
        OpsDataManager.OnNewItemUnlock -= OnNewItemUnlock;
    }
    public void OnNewItemUnlock(MergeItemLevel itemLevel)
    {
        foreach (JokerShopBtn jokerShopBtn in _btnList)
        {
            jokerShopBtn.HandleState();
        }
    }
    public void OnJokerChange(int joker)
    {
        foreach (JokerShopBtn jokerShopBtn in _btnList)
        {
            jokerShopBtn.HandleState();
        }
    }
    // public void ChangeVisibility()
    // {
    //     if (isHidden)
    //         Show();
    //     else
    //         Hide();
    // }
    // private void Hide()
    // {
    //     if (isHidden) return;
    //     StartCoroutine(AncoredMoveCoroutine(new Vector2(_rectTransform.anchoredPosition.x, _rectTransform.rect.height), _aniamtionDuration));
    //     isHidden = true;
    // }
    // private void Show()
    // {

    //     isHidden = false;
    //     StartCoroutine(AncoredMoveCoroutine(new Vector2(_rectTransform.anchoredPosition.x, 0), _aniamtionDuration));
    //     //Invoke(nameof(Hide), 5f);
    // }
    // private IEnumerator AncoredMoveCoroutine(Vector2 endPostion, float duration)
    // {
    //     float tempTime = 0;
    //     Vector2 startPostion = _rectTransform.anchoredPosition;
    //     while (tempTime < _aniamtionDuration)
    //     {
    //         tempTime += Time.deltaTime;
    //         _rectTransform.anchoredPosition = Vector2.Lerp(startPostion, endPostion, tempTime / duration);
    //         yield return null;
    //     }
    // }
    #region Tutorial
    public void StartTutorial()
    {
        foreach (JokerShopBtn btn in _btnList)
        {
            btn.GetComponent<Button>().interactable = false;
        }

        _btnList[1].StartTutorial();

    }
    public void EndTutorial()
    {
        foreach (JokerShopBtn btn in _btnList)
        {
            btn.GetComponent<Button>().interactable = true;
        }
        _btnList[1].EndTutorial();
    }
    #endregion
}
