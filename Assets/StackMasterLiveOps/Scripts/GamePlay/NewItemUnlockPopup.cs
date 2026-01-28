using FunCell.GridMerge.GamePlay;
using FunCell.GridMerge.System;
using FunCell.GridMerge.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class NewItemUnlockPopup : Panel
{
    [SerializeField] private Image _image;
    [SerializeField] private ItemSpriteList _itemSpriteList;
    public TMP_Text TitleTxt;
    public override void Open()
    {
        OpsAnalyticsManager.Instance.LogEvent("liveops:newitem_unlock");
        base.Open();
        switch (OpsDataManager.UnlockedItemLevel)
        {
            case MergeItemLevel.Level1:
                TitleTxt.text = "Mercury UNLOCKED";
                break;
            case MergeItemLevel.Level2:
                TitleTxt.text = "Venus  UNLOCKED";
                break;
            case MergeItemLevel.Level3:
                TitleTxt.text = "Earth UNLOCKED";
                break;
            case MergeItemLevel.Level4:
                TitleTxt.text = "Mars UNLOCKED";
                break;
            case MergeItemLevel.Level5:
                TitleTxt.text = "Neptune UNLOCKED";
                break;
            case MergeItemLevel.Level6:
                TitleTxt.text = "Uranus UNLOCKED";
                break;
            case MergeItemLevel.Level7:
                TitleTxt.text = "Saturn UNLOCKED";
                break;
            case MergeItemLevel.Level8:
                TitleTxt.text = "Jupiter UNLOCKED";
                break;
        }
        Show(_itemSpriteList.sprites[(int)OpsDataManager.UnlockedItemLevel - 1]);
    }
    public override void Close()
    {
        base.Close();
    }
    [ContextMenu("Show")]
    public void Show()
    {
        Show(null);
    }
    public void Show(Sprite sprite)
    {
        if (sprite != null)
            _image.sprite = sprite;
        GetComponent<Animation>().Play();
        LeanTween.delayedCall(3, () =>
        {
            PanelManager.Instance.Open<GamePanel>();
        });
    }

}
