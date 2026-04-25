using Common.MVVM;
using R3;
using UI.GameplayMenu.Models;

namespace UI.GameplayMenu.ViewModels
{
    public class SettingsViewModel : IViewModel
    {
        private SettingsModel _model;

        public Observable<bool> SettingsWindowStateChangedSignal => _model.SettingsWindowStateChangedSignal;
        public Observable<float> SFXVolumeChangedSignal => _model.SFXVolumeChangedSignal;
        public Observable<float> MusicVolumeChangedSignal => _model.MusicVolumeChangedSignal;

        public void BindModel(IModel model)
        {
            _model = model as SettingsModel;
        }

        public void Dispose()
        {
            _model.Dispose();
        }

        public void OpenVK() => _model.OpenVK();
        public void Close() => _model.Close();
        public void SetSFXVolume(float volume) => _model.SetSFXVolume(volume);
        public void SetMusicVolume(float volume) => _model.SetMusicVolume(volume);
        public void ToggleSFXState(bool state) => _model.ToggleSFXState(state);
        public void ToggleMusicState(bool state) => _model.ToggleMusicState(state);
    }
}