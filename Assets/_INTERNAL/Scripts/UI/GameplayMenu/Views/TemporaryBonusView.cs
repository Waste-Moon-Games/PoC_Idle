using UnityEngine;
using UnityEngine.UI;

namespace UI.GameplayMenu.Views
{
    public class TemporaryBonusView : MonoBehaviour
    {
        [SerializeField] private Slider _bonusTimer;
        [SerializeField] private GameObject _contentContainer;

        [Space(5), Header("Visualization Settings")]
        [SerializeField] private Color _startColor;
        [SerializeField] private Color _endColor;
        [SerializeField] private Image _fill;

        private float _startTime;
        private bool _isActive = false;

        private void Start()
        {
            if (_isActive)
                return;

            if (_contentContainer.activeSelf && !_isActive)
                Toggle(false);

            _fill.color = _startColor;
        }

        public void Toggle(bool state)
        {
            _isActive = state;
            _contentContainer.SetActive(state);

            if (!state)
                ResetTimer();
        }

        public void SetStartTime(float startTime)
        {
            _startTime = startTime;
            _bonusTimer.maxValue = startTime;

            if (_isActive)
                return;

            _bonusTimer.value = startTime;
        }

        public void UpdateProgress(float time)
        {
            _bonusTimer.value = time;

            float timer = Mathf.Clamp01(time / _startTime);
            _fill.color = Color.Lerp(_endColor, _startColor, timer);
        }

        private void ResetTimer()
        {
            _fill.color = _startColor;
            _bonusTimer.value = _startTime;
        }
    }
}