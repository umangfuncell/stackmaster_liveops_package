using UnityEngine;
using TMPro;
using System.Collections;
namespace FunCell.GridMerge.GamePlay
{
    public class WorldScoreLable : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text_Lable;
        public void Init(Vector3 position, Quaternion rotation, string text)
        {
            _text_Lable.text = text;
            transform.position = position + Vector3.up;
            transform.rotation = rotation;
            _text_Lable.alpha = 0;
            // transform.localScale = Vector3.zero;
            float delayVal = 0.5f;
            transform.LeanMove(transform.position + (Vector3.up), 0.5f).setDelay(delayVal).setEase(LeanTweenType.easeOutExpo).setOnComplete(() => { Destroy(gameObject); });
            transform.LeanScale(Vector3.one * 0.7f, 0.5f).setEase(LeanTweenType.easeInOutElastic).setDelay(delayVal);
            
            LeanTween.value(gameObject, (float val) =>
            {
                _text_Lable.alpha = val;
            }, 1f, 0f, 0.5f).setEase(LeanTweenType.easeInExpo).setDelay(delayVal);

            LeanTween.value(gameObject, (float val) =>
            {
                _text_Lable.alpha = val;
            }, 0f, 1f, 0.5f).setEase(LeanTweenType.easeInExpo);
            // AlphaChange();
        }
        // private void AlphaChange()
        // {
        //     float time = 0.9f;
        //     _text_Lable.alpha = Mathf.Lerp(1, 0, time);
        //     // while (time < 1)
        //     // {
        //     //     Debug.LogError("" + time);
        //     //     yield return null;
        //     // }
        // }
    }
}

