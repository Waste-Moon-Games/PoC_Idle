using Common.MVVM;
using UnityEngine;
using UI.GameplayMenu.ViewModels;
using UnityEngine.UI;
using R3;
using Core.LevelingSystem;

namespace UI.GameplayMenu.Views
{
    [RequireComponent(typeof(Button))]
    public class GetRewardView: MonoBehaviour, IView
    {
        private readonly CompositeDisposable _disposables = new();

        [SerializeField] private Button _reciveButton;

        private GetRewardViewModel _viewModel;

        private void Start() => _reciveButton.onClick.AddListener(HandleReciveButtonClick);

        private void OnDestroy()
        {
            _reciveButton.onClick.RemoveListener(HandleReciveButtonClick);
            _viewModel?.Dispose();
            _disposables.Dispose();
        }

        private void HandleReciveButtonClick() => _viewModel.TryToReciveThisReward();
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
                case RewardState.Recieved:
                    HandleRecieveState();
                    break;
                default:
                    Debug.Log("Invalid State!");
                    break;
            }
        }

        private void HandleLockedState()
        {
            //to do иконку "закрытой" награды
        }

        private void HandleUnlockedState()
        {
            //to do иконку "открытой" награды
        }

        private void HandleRecieveState()
        {
            //to do иконку "полученной" награды
        }

        public void BindViewModel(IViewModel viewModel)
        {
            _viewModel = viewModel as GetRewardViewModel;

            _viewModel.RequestedRewardState.Subscribe(HandleRewardState).AddTo(_disposables);

            _viewModel.RequestRewardState();
        }
    }
}