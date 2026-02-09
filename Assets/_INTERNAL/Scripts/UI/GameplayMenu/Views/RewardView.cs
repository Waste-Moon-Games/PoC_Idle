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

        [SerializeField] private Button _reciveButton;
        [SerializeField] private GameObject _lock;
        [SerializeField] private GameObject _recieved;
        [SerializeField] private TextMeshProUGUI _descriptionText;

        private RewardViewModel _viewModel;

        private void Start() => _reciveButton.onClick.AddListener(HandleReciveButtonClick);

        private void OnDestroy()
        {
            _reciveButton.onClick.RemoveListener(HandleReciveButtonClick);
            _viewModel?.Dispose();
            _disposables.Dispose();
        }

        private void HandleReciveButtonClick() => _viewModel.TryToReciveThisReward();
        private void HandleRequestedRewardAmount(float amount) => _descriptionText.text = $"{amount}";
        
        private void HandleRewardState(RewardState state)
        {
            Debug.Log($"Reward state: {state}");

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
            if(!_lock.activeSelf)
                _lock.SetActive(true);
        }

        private void HandleUnlockedState()
        {
            if(_lock.activeSelf)
                _lock.SetActive(false);
        }

        private void HandleReceiveState()
        {
            if(!_recieved.activeSelf && !_lock.activeSelf)
            {
                _lock.SetActive(false);
                _recieved.SetActive(true);
            }
        }

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as RewardViewModel;

            _viewModel.RequestedRewardState.Subscribe(HandleRewardState).AddTo(_disposables);
            _viewModel.RewardAmountSignal.Subscribe(HandleRequestedRewardAmount).AddTo(_disposables);

            _viewModel.RequestRewardState();
        }
    }
}