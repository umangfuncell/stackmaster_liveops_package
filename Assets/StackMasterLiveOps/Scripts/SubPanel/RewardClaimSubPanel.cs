using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using FunCell.GridMerge.GamePlay;
using Spine.Unity;
using FunCell.GridMerge.UI;

public class RewardClaimSubPanel : MonoBehaviour
{
    [SerializeField] private Button _close_Btn;

    [SerializeField] private RewardFrame _rewardFrame;
    [SerializeField] private Transform _layoutParent;
    [SerializeField] private SkeletonGraphic _chest;
    [SerializeField] private Button _reward_Btn;
    [SerializeField] private TMP_Text _noThanks_Label;
    [SerializeField] private Transform _glow;

    [Header("Test Variable")]
    public float startScale;
    public float moveAnimationSpeed;
    public float animationStartDely;
    public float delayBetweenTwoItems;

    [Space]
    [SerializeField] private OpsRewardChest _testChest;
    public void Open(OpsRewardChest chest)
    {
        ClearChildren(_layoutParent);
        OpsGameManager.Instance.ChangeStateTo(GameState.Pause);

        _close_Btn.gameObject.SetActive(false);
        _noThanks_Label.gameObject.SetActive(false);
        _reward_Btn.gameObject.SetActive(false);
        _chest.gameObject.SetActive(false);
        _glow.gameObject.SetActive(false);

        _close_Btn.onClick.AddListener(OnCloseBtn);
        _reward_Btn.onClick.AddListener(OnRewardBtn);

        gameObject.SetActive(true);
        transform.localScale = Vector3.one * 0.5f;
        transform.LeanScale(Vector3.one, 0.5f).setOnComplete(() =>
        {
            LeanTween.delayedCall(0.3f, () => PlayAnimation());
            LeanTween.delayedCall(0.3f + animationStartDely, () =>
            {
                if (!isActiveAndEnabled)
                {
                    var finalFrames = new List<RewardFrame>();

                    foreach (var reward in chest.rewards)
                    {
                        var frame = Instantiate(_rewardFrame, _layoutParent);
                        frame.SetData(reward);
                        frame.Show();
                        finalFrames.Add(frame);
                    }
                    _close_Btn.gameObject.SetActive(true);
                    _noThanks_Label.gameObject.SetActive(true);
                    _reward_Btn.gameObject.SetActive(true);
                    return;
                }

                StartCoroutine(SpawnItem(chest));
            });

        });
    }
    public void HandleFail()
    {

    }
    [ContextMenu("Open")]
    public void Open()
    {
        ClearChildren(_layoutParent);
        Open(_testChest);
    }
    public void Close()
    {
        OpsGameManager.Instance.ChangeStateTo(GameState.Resume);

        _close_Btn.onClick.RemoveListener(OnCloseBtn);
        _reward_Btn.onClick.RemoveListener(OnRewardBtn);

        gameObject.SetActive(false);
    }
    private void OnCloseBtn()
    {
        Close();
    }
    [ContextMenu("PlayAnimation")]
    public void PlayAnimation()
    {
        _chest.gameObject.SetActive(true);
        _glow.gameObject.SetActive(true);
        _chest.AnimationState.SetAnimation(0, "animation F", false);
        _glow.localScale = Vector3.one * 0.1f;
        _glow.LeanScale(Vector3.one, 0.5f);
    }
    public void ClearLayout()
    {

    }

    public IEnumerator SpawnItem(OpsRewardChest chest)
    {
        var finalFrames = new List<RewardFrame>();

        foreach (var reward in chest.rewards)
        {
            var frame = Instantiate(_rewardFrame, _layoutParent);
            frame.SetData(reward);
            frame.Hide();
            finalFrames.Add(frame);
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            _layoutParent.GetComponent<RectTransform>()
        );

        var chestRect = _chest.GetComponent<RectTransform>();
        var flyParent = GetComponent<RectTransform>();

        for (int i = 0; i < finalFrames.Count; i++)
        {
            var targetFrame = finalFrames[i];
            var targetRect = targetFrame.GetComponent<RectTransform>();

            var flying = Instantiate(_rewardFrame, flyParent);
            var flyingRect = flying.GetComponent<RectTransform>();

            flyingRect.SetAsLastSibling();

            flying.SetData(chest.rewards[i]);

            flyingRect.anchorMin = flyingRect.anchorMax = new Vector2(0.5f, 0.5f);
            flyingRect.pivot = new Vector2(0.5f, 0.5f);

            flyingRect.anchoredPosition =
                WorldToAnchored(chestRect, flyParent);

            flyingRect.localScale = Vector3.one * startScale;

            LeanTween.scale(flyingRect, Vector3.one, moveAnimationSpeed);
            LeanTween.move(
                flyingRect,
                WorldToAnchored(targetRect, flyParent),
                moveAnimationSpeed
            )
            .setEaseOutExpo()
            .setOnComplete(() =>
            {
                targetFrame.Show();
                Destroy(flying.gameObject);
            });

            yield return new WaitForSeconds(delayBetweenTwoItems);
        }
        yield return new WaitForSeconds(0.3f);
        _close_Btn.gameObject.SetActive(true);
        _noThanks_Label.gameObject.SetActive(true);
        _reward_Btn.gameObject.SetActive(true);
    }
    Vector2 WorldToAnchored(RectTransform target, RectTransform parent)
    {
        Vector2 screenPoint =
            RectTransformUtility.WorldToScreenPoint(null, target.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parent,
            screenPoint,
            null,
            out Vector2 localPoint
        );

        return localPoint;
    }
    void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }
    void OnRewardBtn()
    {
        OpsAdManager.Instance.ShowRewardedAd(OnRewardSuccess, null, "");
    }
    void OnRewardSuccess()
    {

    }
}
