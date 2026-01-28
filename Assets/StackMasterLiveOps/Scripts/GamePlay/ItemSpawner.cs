using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

namespace FunCell.GridMerge.GamePlay
{
    public class ItemSpawner : MonoBehaviour
    {
        [SerializeField] private List<SpawnerSlot> _slots;
        [SerializeField] private Transform[] _slotsPosition;
        [SerializeField] private float _enterExitAnimationTime = 3;
        [SerializeField] private Transform _highlightParticleTransform;
        [SerializeField] private float _animationTime = 0.4f;
        private MergeItem _item;

        public event Action OnSpawnerChanged;

        public void Init(MergeItem mergeItem)
        {
            gameObject.SetActive(false);
            _item = mergeItem;

            foreach (SpawnerSlot slot in _slots)
                slot.Init(this);
            transform.position = new Vector3(transform.position.x, -5, transform.position.z);
            _highlightParticleTransform.gameObject.SetActive(false);

        }
        public void StartNewGame(bool canAnimate = false)
        {
            gameObject.SetActive(true);

            for (int i = 0; i < 3; i++)
            {
                Vector3 position = _slots[i].transform.localPosition;
                position.y = 0;
                _slots[i].transform.localPosition = position;
            }

            if (canAnimate)
            {
                transform.LeanMoveY(0, _enterExitAnimationTime).setOnComplete(() =>
                {
                    SetItems();
                });
            }
            else
            {
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                SetItems();
            }
        }
        private void SetItems()
        {
            for (int i = 0; i < 3; i++)
            {
                _slots[i].SetItem(CreateRandomMergeItem());
                _slots[i].PopDown();
            }
            _highlightParticleTransform.gameObject.SetActive(true);
            _slots[0].GetItem().ChangeDrgeTo(true);
        }
        public void LoadOldItems(MergeItemLevel[] oldItems, bool canAnimate = false)
        {
            gameObject.SetActive(true);
            if (canAnimate)
            {
                transform.LeanMoveY(0, _enterExitAnimationTime).setOnComplete(() =>
                {
                    SetOldItems(oldItems);
                });
            }
            else
            {
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                SetOldItems(oldItems);
            }

        }
        private void SetOldItems(MergeItemLevel[] oldItems)
        {
            for (int i = 0; i < 3; i++)
            {
                MergeItem item = Instantiate(_item);
                item.SetLevel(oldItems[i]);
                _slots[i].SetItem(item);
                _slots[i].PopDown();
            }
            _highlightParticleTransform.gameObject.SetActive(true);
            _slots[0].GetItem().ChangeDrgeTo(true);
        }
        public MergeItemLevel[] GetItems()
        {
            MergeItemLevel[] itemLevels = new MergeItemLevel[3];
            for (int i = 0; i < 3; i++)
            {
                itemLevels[i] = _slots[i].GetItem().ItemLevel;
            }
            return itemLevels;
        }
        private MergeItem CreateRandomMergeItem()
        {
            MergeItem item = Instantiate(_item);
            MergeItemLevel level = MergeItem.GetRandomLevel();
            item.SetLevel(level);
            return item;
        }


        // for normal slide animation
        //public void RemoveFirstItem()
        //{
        //    _slots[0].ClearItem();

        //    for (int i = 0; i < _slots.Count - 1; i++)
        //    {
        //        MergeItem nextItem = _slots[i + 1].GetItem();
        //        _slots[i + 1].ClearItem();

        //        if (nextItem != null)
        //        {
        //            _slots[i].SetItem(nextItem);
        //            _slots[i].Slide();
        //        }
        //    }

        //    MergeItem newItem = CreateRandomMergeItem();
        //    _slots[_slots.Count - 1].SetItem(newItem);
        //    _slots[_slots.Count - 1].PopDown();
        //}

        public void RemoveFirstItem(Action onComplate)
        {
            SpawnerSlot firstSlot = _slots[0];
            firstSlot.RemoveItem();
            _slots.RemoveAt(0);

            MergeItem newItem = CreateRandomMergeItem();
            _slots[_slots.Count - 1].SetItem(newItem);
            newItem.ChangeDrgeTo(false);
            firstSlot.transform.LeanMoveY(-25, _animationTime);
            _highlightParticleTransform.gameObject.SetActive(false);
            for (int i = 0; i < _slots.Count - 1; i++)
            {
                //_slots[i].transform.LeanMoveX(_slotsPosition[i].transform.position.x, _animationTime);
                _slots[i].transform.LeanMove(_slotsPosition[i].transform.position, _animationTime);
                _slots[i].transform.LeanScale(_slotsPosition[i].transform.localScale, _animationTime);
            }
            _slots[_slots.Count - 1].transform.LeanScale(_slotsPosition[2].localScale, _animationTime);
            _slots[_slots.Count - 1].transform.LeanMove(_slotsPosition[2].position, _animationTime).setOnComplete(() =>
            {
                _slots.Add(firstSlot);
                Vector3 newPosition = _slots[_slots.Count - 2].transform.position;
                newPosition.y = -25;
                firstSlot.transform.position = newPosition;
                GetFirstItem().ChangeDrgeTo(true);
                onComplate?.Invoke();
                _highlightParticleTransform.gameObject.SetActive(true);
            });
        }
        public MergeItem GetFirstItem()
        {
            return _slots[0].GetItem();
        }
        public void ChangeFirstItem(MergeItemLevel level)
        {
            _slots[0].GetItem().SetLevel(level);
            _slots[0].PopDown();
            OnSpawnerChanged?.Invoke();
        }
        public void UnLoad(bool canAnimate = false)
        {
            _highlightParticleTransform.gameObject.SetActive(false);
            if (canAnimate)
            {
                transform.LeanMoveY(-5, _enterExitAnimationTime).setOnComplete(() =>
                {
                    gameObject.SetActive(false);
                    for (int i = 0; i < 3; i++)
                    {
                        Destroy(_slots[i].GetItem().gameObject);
                        _slots[i].RemoveItem();
                    }
                });
            }
            else
            {
                gameObject.SetActive(false);
                transform.position = new Vector3(transform.position.x, -5, transform.position.z);
                for (int i = 0; i < 3; i++)
                {
                    Destroy(_slots[i].GetItem().gameObject);
                    _slots[i].RemoveItem();
                }
            }


            //for (int i = 0; i < 3; i++)
            //{
            //    _slots[i].GetItem().GoDown();
            //}
        }

        public void StartTutorial(List<MergeItemLevel> mergeItemLevels)
        {
            gameObject.SetActive(true);

            for (int i = 0; i < 3; i++)
            {
                Vector3 position = _slots[i].transform.localPosition;
                position.y = 0;
                _slots[i].transform.localPosition = position;
            }

            transform.LeanMoveY(0, _enterExitAnimationTime).setOnComplete(() =>
            {
                CreateTutorialItem(mergeItemLevels);
                _highlightParticleTransform.gameObject.SetActive(true);
            });
        }
        private void CreateTutorialItem(List<MergeItemLevel> mergeItemLevels)
        {
            for (int i = 0; i < _slots.Count - 1; i++)
            {
                MergeItem item = Instantiate(_item);
                item.Init(mergeItemLevels[i]);
                _slots[i].SetItem(item);
                _slots[i].PopDown();
            }
        }
    }
}
