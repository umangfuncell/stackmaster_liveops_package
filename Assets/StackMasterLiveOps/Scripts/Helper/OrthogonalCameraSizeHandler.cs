using UnityEngine;
namespace FunCell.Helper
{
    public class OrthogonalCameraSizeHandler : MonoBehaviour
    {
        [SerializeField] private float _referenceWidth = 1920f;
        [SerializeField] private float _referenceHeight = 1080f;
        [SerializeField] private bool _isPortrait;
        [SerializeField] private float _referenceOrthoSize = 0f;

        private void Awake()
        {
            if (_isPortrait)
            {
                float temp = _referenceWidth;
                _referenceWidth = _referenceHeight;
                _referenceHeight = temp;
            }
            SetCameraSize();
        }
#if UNITY_EDITOR
        private void Update()
        {
            SetCameraSize();
        }
#endif
        private void SetCameraSize()
        {
            float targetAspect = _referenceWidth / _referenceHeight;
            float currentAspect = (float)Screen.width / Screen.height;
            float size = _referenceOrthoSize * (targetAspect / currentAspect);
            Camera.main.orthographicSize = size;
        }
    }
}

