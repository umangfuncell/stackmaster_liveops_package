using UnityEngine;
using System.Collections;

namespace FunCell.GridMerge.GamePlay
{
    public class SpawnerSlot : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;

        private ItemSpawner _spawner;
        private MergeItem _item;

        public void Init(ItemSpawner spawner)
        {
            _spawner = spawner;
        }
        public void SetItem(MergeItem item)
        {
            _item = item;
            _item.transform.parent = transform;
            //for new animation
            _item.transform.position = _spawnPoint.position;
            _item.transform.localRotation = Quaternion.identity;
        }
        public void RemoveItem()
        {
            if (_item == null) return;
            _item.transform.parent = null;
            _item = null;
        }
        public void PopDown()
        {
            _item.transform.position=_spawnPoint.position + (Vector3.up * 0.5f);
            _item.transform.localRotation = Quaternion.identity;
            _item.transform.LeanMove(_spawnPoint.position, 0.4f);
        }
        public void Slide()
        {
            float distance = Vector3.Distance(_item.transform.position, _spawnPoint.position);
            float duration = distance * 0.2f; // adjust speed here
            _item.transform.LeanMove(_spawnPoint.position, duration).setEaseOutQuad();
        }
        public MergeItem GetItem()
        {
           return _item; 
        }
        public void MoveToSpawn()
        {
            _item.transform.LeanMoveY(_spawnPoint.position.y, 0.5f);
        }
    }
}

