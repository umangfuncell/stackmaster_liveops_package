using UnityEngine;
using System;
using System.Collections;

namespace FunCell.GridMerge.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Animation))]
    public class Panel : MonoBehaviour
    {
        [SerializeField] protected Animation _animation;
        [SerializeField] protected CanvasGroup _canvasGroup;
        [SerializeField] protected AnimationClip _open_AnimationClip;
        [SerializeField] protected AnimationClip _close_AnimationClip;
        private Coroutine _aniamtionCoroutine;
        [ContextMenu("SetPanel")]
        public void SetPanel()
        {
            _animation = GetComponent<Animation>();
            _canvasGroup = GetComponent<CanvasGroup>();

            _canvasGroup.interactable = false;
            _animation.playAutomatically = false;
            _animation.AddClip(_open_AnimationClip, _open_AnimationClip.name);
            _animation.AddClip(_close_AnimationClip, _close_AnimationClip.name);
        }

        public virtual void Open()
        {
            _canvasGroup.interactable = false;
            gameObject.SetActive(true);
            if (_open_AnimationClip != null)
            {
                PlayAnimation(_open_AnimationClip, () => OnOpenAnimationFinish());
            }
            else
            {
                OnOpenAnimationFinish();
            }
        }
        public virtual void Close()
        {
            _canvasGroup.interactable = false;
            if (_close_AnimationClip != null)
            {
                PlayAnimation(_close_AnimationClip, () => OnCloseAnimationFinish());
            }
            else
            {
                OnCloseAnimationFinish();
            }
        }

        protected virtual void OnOpenAnimationFinish()
        {
            _canvasGroup.interactable = true;
        }
        protected virtual void OnCloseAnimationFinish()
        {
            _canvasGroup.interactable = false;
            gameObject.SetActive(false);
        }
        protected IEnumerator AnimationCoroutine(float wait, Action action)
        {
            yield return new WaitForSeconds(wait);
            _aniamtionCoroutine = null;
            action?.Invoke();
        }
        protected void PlayAnimation(AnimationClip clip, Action action)
        {
            if (_aniamtionCoroutine != null)
            {
                StopCoroutine(_aniamtionCoroutine);
                _aniamtionCoroutine = null;
            }
            _aniamtionCoroutine = StartCoroutine(AnimationCoroutine(clip.length, action));
            _animation.Play(clip.name);
        }
    }
}

