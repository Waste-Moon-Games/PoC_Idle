using DG.Tweening;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameplayMenu.Views.BonusesFromRewardAd
{
    public class RewardFromAdBonusInfoView : MonoBehaviour
    {
        [SerializeField] private float _toggleAnimationDuration = 0.25f;
        [SerializeField] private TextMeshProUGUI _bonusDescription;

        [Space(5), Header("Buttons")]
        [SerializeField] private Button _showAdButton;
        [SerializeField] private Button _closeButton;

        private readonly Subject<Unit> _showAdButtonClickSignal = new();
        private readonly Subject<Unit> _closeButtonClickSignal = new();

        public Observable<Unit> ShowAdButtonClickSingal => _showAdButtonClickSignal.AsObservable();
        public Observable<Unit> CloseButtonClickSignal => _closeButtonClickSignal.AsObservable();

        private void Start()
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                transform.DOScale(0f, _toggleAnimationDuration);
            }

            if(_showAdButton == null || _closeButton == null)
            {
                Debug.LogWarning("[Reward From Ad Bonus Info View] Some button is empty!");
                return;
            }

            _showAdButton.onClick.AddListener(HandleShowAdButtonClick);
            _closeButton.onClick.AddListener(Close);
        }

        private void OnDestroy()
        {
            if (_showAdButton == null || _closeButton == null)
            {
                Debug.LogWarning("[Reward From Ad Bonus Info View] Some button is empty!");
                return;
            }

            _showAdButton.onClick.RemoveListener(HandleShowAdButtonClick);
            _closeButton.onClick.RemoveListener(Close);
        }

        public void SetDescription(string desc) => _bonusDescription.text = desc;

        public void Open()
        {
            gameObject.SetActive(true);
            transform.DOScale(1f, _toggleAnimationDuration).SetEase(Ease.OutQuad);
        }

        private void Close()
        {
            transform.DOScale(0f, _toggleAnimationDuration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                gameObject.SetActive(false);
                _closeButtonClickSignal.OnNext(Unit.Default);
            });
        }

        private void HandleShowAdButtonClick() => _showAdButtonClickSignal.OnNext(Unit.Default);
    }
}