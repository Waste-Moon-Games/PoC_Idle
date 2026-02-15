using Common.MVVM;
using UnityEngine;
using UI.GameplayMenu.ViewModels;
using UnityEngine.UI;
using R3;
using Core.LevelingSystem;
using TMPro;

namespace UI.GameplayMenu.Views
{
    [RequireComponent(typeof(Button))]
    public class RewardView: MonoBehaviour, IView
    {
        private readonly CompositeDisposable _disposables = new();

        [SerializeField] private TextMeshProUGUI _requiredLevelText;
        [SerializeField] private Button _reciveButton;
        [SerializeField] private GameObject _lockState;
        [SerializeField] private GameObject _recievedState;
        [SerializeField] private GameObject _connector;
        [SerializeField] private TextMeshProUGUI _amountText;

        private RewardViewModel _viewModel;

        private void Start() => _reciveButton.onClick.AddListener(HandleReciveButtonClick);

        private void OnDestroy()
        {
            _reciveButton.onClick.RemoveListener(HandleReciveButtonClick);
            _viewModel?.Dispose();
            _disposables.Dispose();
        }

        private void HandleReciveButtonClick() => _viewModel.TryToReciveThisReward();
        private void HandleRequestedRewardAmount(float amount) => _amountText.text = $"{amount}";
        
        private void HandleRewardState(RewardState state)
        {
            switch(state)
            {
                case RewardState.Locked:
                    HandleLockedState();
                    break;
                case RewardState.Unlocked:
                    HandleUnlockedState();
                    break;
                case RewardState.Received:
                    HandleReceiveState();
                    break;
                default:
                    Debug.Log("Invalid State!");
                    break;
            }
        }

        private void HandleLockedState()
        {
            if(!_lockState.activeSelf)
                _lockState.SetActive(true);
        }

        private void HandleUnlockedState()
        {
            if(_lockState.activeSelf)
                _lockState.SetActive(false);
        }

        private void HandleReceiveState()
        {
            if(!_recievedState.activeSelf && !_lockState.activeSelf)
            {
                _lockState.SetActive(false);
                _recievedState.SetActive(true);
            }
        }

        private void HandleConnectorState(bool isLast) => _connector.SetActive(!isLast);

        private void HandleRewardRequiredLevel(int level) => _requiredLevelText.text = $"{level}";

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as RewardViewModel;

            _viewModel.RequestedRewardState.Subscribe(HandleRewardState).AddTo(_disposables);
            _viewModel.RewardAmountSignal.Subscribe(HandleRequestedRewardAmount).AddTo(_disposables);
            _viewModel.RewardRequierdLevelSignal.Subscribe(HandleRewardRequiredLevel).AddTo(_disposables);
            _viewModel.ConnectorStateSignal.Subscribe(HandleConnectorState).AddTo(_disposables);

            _viewModel.RequestRewardState();
        }
    }
}