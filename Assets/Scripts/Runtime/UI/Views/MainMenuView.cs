using ColbyO.VNTG.PSX;
using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;
using WrongFloor.MonoSystems;
using WrongFloor.UI;

namespace WrongFloor
{
    public class MainMenuView : View
    {
        [SerializeField] private GameObject _mainMenuBackground;

        [SerializeField] protected GameObject _settingBackdrop;

        [SerializeField] private EventButton _play;
        [SerializeField] private EventButton _settings;
        [SerializeField] private EventButton _quit;

        private IVisualEffectMonoSystem _visualMS;
        private ILightMonoSystem _lightMS;

        private void Play()
        {
            WFGameManager.HideCursor();
            _visualMS.FadeOut(2f).Then(_ =>
            {
                _visualMS.FadeIn(5f);
                _mainMenuBackground.SetActive(false);
                _settingBackdrop.SetActive(true);
                PSXEffectSettings psxSettings = _visualMS.GetPSXSettings();
                psxSettings.EnableFog.value = false; ;
                _lightMS.SetColor(null, LightState.Emergency);
                WFGameManager.IsPaused = false;
                GameManager.GetMonoSystem<IUIMonoSystem>().Show<GameView>();
            });
        }

        private void Settings()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<SettingsView>();
        }

        private void Quit()
        {
            Application.Quit();
        }

        public override void Show()
        {
            base.Show();
            WFGameManager.ShowCursor();
            WFGameManager.IsPaused = true;
            _mainMenuBackground.SetActive(true);
            _settingBackdrop.SetActive(false);

            PSXEffectSettings psxSettings = _visualMS.GetPSXSettings();
            psxSettings.EnableFog.value = true;

            _lightMS.SetColor(Color.red, LightState.Emergency);
        }

        public override void Init()
        {
            _lightMS = GameManager.GetMonoSystem<ILightMonoSystem>();
            _visualMS = GameManager.GetMonoSystem<IVisualEffectMonoSystem>();

            _play.onPointerDown.AddListener(Play);
            _settings.onPointerDown.AddListener(Settings);
            _quit.onPointerDown.AddListener(Quit);
        }
    }
}
