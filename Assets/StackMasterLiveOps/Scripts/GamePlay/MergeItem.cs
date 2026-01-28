using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Collections;
using FunCell.Helper;
using FunCell.GridMerge.System;
namespace FunCell.GridMerge.GamePlay
{
    public class MergeItem : MonoBehaviour
    {
        [SerializeField] private MergeItemLevel _itemLevel;
        [SerializeField] private List<GameObject> _levelVisuals;
        [SerializeField] private int[] _socre;
        [SerializeField] private SpriteRenderer _childSpriteRenderer;// assing runtime when neede
        [SerializeField] private ParticleSystem _dustParticle;
        [SerializeField] private float _curveMoveAnimationSpeed = 0.3f;

        [SerializeField] private float _bounceHeight = 0.2f;
        [SerializeField] private float _bounceDuration = 0.15f;
        [SerializeField] private float _popDownDuration = 0.2f;
        [SerializeField] private float _squeezDurationPerStage = 0.07f;

        [SerializeField] private bool _mergePlayOnly;

        [Header("GOTO Grid Animation Curve")]
        [SerializeField] private Vector3 _gotoGridScale;
        [SerializeField] private float _moveToGridAnimationSpeed = 0.3f;
        [SerializeField] private float _curveAmount_MoveToGrid = 5;

        [Header("GOTO Grid Animation Curve")]
        [SerializeField] private float _moveToGridLinearAnimationSpeed = 0.3f;


        [Header("Sfx")]
        [SerializeField] private AudioClip _gotoGridNodeSfx;
        [SerializeField] private AudioClip _mergeSfx;
        [SerializeField] private AudioClip _mergeShortSfx;



        public MergeItemLevel ItemLevel => _itemLevel;

        public void Init(MergeItemLevel startingLevel)
        {
            SetLevel(startingLevel);
        }
        public void SetLevel(MergeItemLevel newLevel)
        {
            _itemLevel = newLevel;

            for (int i = 0; i < _levelVisuals.Count; i++)
            {
                _levelVisuals[i].SetActive(i == (int)_itemLevel - 1);
            }
        }
        public void Upgrade()
        {
            OpsSoundManager.Instance.PlayMergeSfx();
            OpsHapticManger.Instance.MediumHapticTouch();
            int nextLevel = (int)_itemLevel + 1;


            if (_mergePlayOnly)
            {
                if (_dustParticle != null)
                    _dustParticle.Play();
            }

            PlayDustParticle();


            if (nextLevel > _levelVisuals.Count)
            {
                return;
            }
            if (OpsDataManager.UnlockedItemLevel < (MergeItemLevel)nextLevel)
            {
                OpsGameManager.Instance.UnlockNewItem((MergeItemLevel)nextLevel);
            }
            SetLevel((MergeItemLevel)nextLevel);
        }
        public int GetScore()
        {
            switch (_itemLevel)
            {
                case MergeItemLevel.Level1: return _socre[0];
                case MergeItemLevel.Level2: return _socre[1];
                case MergeItemLevel.Level3: return _socre[2];
                case MergeItemLevel.Level4: return _socre[3];
                case MergeItemLevel.Level5: return _socre[4];
                case MergeItemLevel.Level6: return _socre[5];
                case MergeItemLevel.Level7: return _socre[6];
                default: return _socre[7];
            }
        }
        public void PopDown(Vector3 downPosition, Action onComplete)
        {
            OpsSoundManager.Instance.PlaySfx(_gotoGridNodeSfx);
            OpsHapticManger.Instance.MediumHapticTouch();
            transform.LeanMove(downPosition, _popDownDuration).setOnComplete(() =>
            {
                onComplete?.Invoke();
                PlayDustParticle();
            });
        }
        public void GotoGridNode(Vector3 downPosition, Action onComplete)
        {
            ChangeDrgeTo(false);

            OpsSoundManager.Instance.PlayGoToNodeSfx();
            OpsHapticManger.Instance.MediumHapticTouch();

            //Curve Movement
            Vector3[] positions = MergeGrid.GetItemMovePath(transform.position, downPosition, 5);
            LeanTween.move(gameObject, positions, _moveToGridAnimationSpeed).setEase(LeanTweenType.linear).
            setOnComplete(() =>
            {
                onComplete?.Invoke();
                PlayDustParticle();
            });
            transform.LeanScale(_gotoGridScale, _moveToGridAnimationSpeed / 2).setOnComplete(() =>
              {
                  transform.LeanScale(Vector3.one, _moveToGridAnimationSpeed / 2);
              });
        }
        public void GotoGridNodeLinear(Vector3 downPosition, Action onComplete)
        {

            ChangeDrgeTo(false);
            OpsSoundManager.Instance.PlayGoToNodeSfx();
            OpsHapticManger.Instance.MediumHapticTouch();
            //linear movement
            transform.LeanMove(downPosition, _moveToGridLinearAnimationSpeed).setOnComplete(() =>
            {
                onComplete?.Invoke();
                PlayDustParticle();
            });
            transform.LeanScale(_gotoGridScale, _moveToGridLinearAnimationSpeed / 2).setOnComplete(() =>
            {
                transform.LeanScale(Vector3.one, _moveToGridLinearAnimationSpeed / 2);
            });
        }
        public void CurveMove(Vector3[] positions, Action onComplete)
        {
            float distance = Vector3.Distance(positions[0], positions[3]);
            float duration = distance * _curveMoveAnimationSpeed;
            LeanTween.move(gameObject, positions, duration).setEase(LeanTweenType.easeInQuad).
                   setOnComplete(() =>
                   {
                       onComplete?.Invoke();
                   });
        }
        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;
        }
        public void GoDown()
        {
            transform.LeanMoveY(-4.5f, 2.9f);
            //StartCoroutine(FadeOutAlpha(2));
        }
        public void PlayDustParticle()
        {
            if (!_mergePlayOnly)
            {
                if (_dustParticle != null)
                    _dustParticle.Play();
            }
            //animate grid node for impacte of the place item

            GridNodeImpactAniamtion();
            //call camera shake
            // OpsGameManager.Instance.ShakeCamera();
            SqueezingAnimation();
        }
        private void SqueezingAnimation()
        {
            transform.LeanScale(new Vector3(1.2f, 0.7f, 1.2f), _squeezDurationPerStage).setOnComplete(() =>
            {
                transform.LeanScale(new Vector3(0.8f, 1.3f, 0.8f), _squeezDurationPerStage).setOnComplete(() =>
                {
                    transform.LeanScale(new Vector3(1f, 1f, 1f), _squeezDurationPerStage);
                });
            });
        }
        private void GridNodeImpactAniamtion()
        {
            if (transform.parent == null) return;
            float targetY = transform.parent.localPosition.y + _bounceHeight;

            LeanTween.moveY(transform.parent.gameObject, targetY, _bounceDuration).setEase(LeanTweenType.easeOutExpo).setOnComplete(() =>
            {
                targetY = transform.parent.localPosition.y - _bounceHeight;
                LeanTween.moveY(transform.parent.gameObject, targetY, _bounceHeight).setEase(LeanTweenType.easeInExpo);
            });
        }
        public void ChangeDrgeTo(bool value)
        {
            foreach (GameObject obj in _levelVisuals)
            {
                obj.GetComponent<Collider>().enabled = value;
            }
        }
        public void SetOnSpawn(Vector3 startPosition, Vector3 endPosition, Quaternion rotation, float duration)
        {
            ChangeDrgeTo(false);
            transform.position = startPosition;
            transform.localRotation = rotation;
            transform.LeanMove(endPosition, duration);
        }
        public void SetRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }
        private IEnumerator FadeOutAlpha(float duration)
        {
            _childSpriteRenderer = GetComponentInChildren<SpriteRenderer>();

            Color startColor = _childSpriteRenderer.color;
            float startAlpha = startColor.a;

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;

                float newAlpha = Mathf.Lerp(startAlpha, 0f, t);

                _childSpriteRenderer.color = new Color(
                    startColor.r,
                    startColor.g,
                    startColor.b,
                    newAlpha
                );

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            _childSpriteRenderer.color = new Color(
                startColor.r,
                startColor.g,
                startColor.b,
                0f
            );

        }
        public static MergeItemLevel GetRandomLevel()
        {
            int roll = Random.Range(0, 100);

            if (OpsDataManager.UnlockedItemLevel == MergeItemLevel.Level4)
            {
                if (roll < 60) return MergeItemLevel.Level1;      // 60%
                else if (roll < 85) return MergeItemLevel.Level2; // 25%
                else if (roll < 95) return MergeItemLevel.Level3; // 10%
                else return MergeItemLevel.Level4; // 5%
            }
            else
            {
                if (roll < 65) return MergeItemLevel.Level1;      // 65%
                else if (roll < 90) return MergeItemLevel.Level2; // 25%
                else return MergeItemLevel.Level3; // 10%
            }


            //int roll = Random.Range(0, 100);

            //if (roll < 40) return MergeItemLevel.Level1;      // 0–39 → 40%
            //else if (roll < 65) return MergeItemLevel.Level2; // 40–64 → 25%
            //else if (roll < 80) return MergeItemLevel.Level3; // 65–79 → 15%
            //else if (roll < 90) return MergeItemLevel.Level4; // 80–89 → 10%
            //else if (roll < 96) return MergeItemLevel.Level5; // 90–95 → 6%
            //else if (roll < 99) return MergeItemLevel.Level6; // 96–98 → 3%
            //else return MergeItemLevel.Level7;                // 99–100 → 1%
        }
    }
    public enum MergeItemLevel
    {
        None,
        Level1,
        Level2,
        Level3,
        Level4,
        Level5,
        Level6,
        Level7,
        Level8
    }
}
