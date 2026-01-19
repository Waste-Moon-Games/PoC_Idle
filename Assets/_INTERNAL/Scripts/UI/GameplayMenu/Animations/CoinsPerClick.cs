using R3;
using TMPro;
using UnityEngine;

namespace UI.GameplayMenu.Animations
{
    public class CoinsPerClick : MonoBehaviour
    {
        private readonly Subject<Unit> _animationEndedSignal = new();

        [SerializeField] private RectTransform _position;
        [SerializeField] private float _upSpeed;
        [SerializeField] private float _fadeTime;
        [SerializeField] private float _delayBeforeShutdown;
        [SerializeField] private TextMeshProUGUI _coinsPerClick;

        private Vector2 _startPosition;
        private bool _isActive = false;

        public Observable<Unit> AnimationEnded => _animationEndedSignal.AsObservable();

        private void Awake()
        {
            _position = GetComponent<RectTransform>();
        }

        private void Start()
        {
            _startPosition = transform.position;
        }

        private void OnDisable()
        {
            transform.position = _startPosition;
            _fadeTime = _delayBeforeShutdown;
            _coinsPerClick.alpha = 100f;
        }

        private void Update()
        {
            if (_isActive)
                MoveUp();
        }

        public void SetCoinsPerClickAmount(string amount)
        {
            _isActive = true;
            _coinsPerClick.text = $"+<color=#00FFFF>{amount}</color> PoCs";
        }

        public void SetupPosition(Vector2 position)
        {
            _position.anchoredPosition = position;
        }

        private void MoveUp()
        {
            transform.Translate(_upSpeed * Time.deltaTime * Vector3.up);
            Fade();
        }

        private void Fade()
        {
            _fadeTime -= Time.deltaTime;

            float duration = _fadeTime / _delayBeforeShutdown;

            _coinsPerClick.alpha = duration;

            if (_fadeTime < 0)
            {
                _isActive = false;
                _animationEndedSignal.OnNext(Unit.Default);
            }
        }
    }
}