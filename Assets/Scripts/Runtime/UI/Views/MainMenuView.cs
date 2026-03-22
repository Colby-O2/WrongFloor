using ColbyO.VNTG.PSX;
using PlazmaGames.Core;
using PlazmaGames.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using WrongFloor.MonoSystems;
using WrongFloor.UI;

namespace WrongFloor
{
    public class MainMenuView : View
    {
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private GameObject _mainMenuBackground;
        [SerializeField] private GameObject _mainMenuScene;

        [SerializeField] protected GameObject _settingBackdrop;

        [SerializeField] private EventButton _play;
        [SerializeField] private EventButton _settings;
        [SerializeField] private EventButton _quit;

        private float _originalMusicVolume = 1.0f;

        private Coroutine _musicFadeRoutine;

        public bool HasBeatGame = false;

        private IVisualEffectMonoSystem _visualMS;
        private ILightMonoSystem _lightMS;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoad;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoad;
        }

        private void Awake()
        {
            _originalMusicVolume = _musicSource.volume;
        }

        private void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            if (HasBeatGame)
            {
                _lightMS.Toggle(LightState.Emergency);
                _lightMS.SetColor(Color.red, LightState.Emergency);
                _lightMS.SetIntensity(1f, LightState.Emergency);

                PSXEffectSettings psxSettings = _visualMS.GetPSXSettings();
                psxSettings.EnableFog.value = true;

                if (_musicSource) _musicSource.volume = _originalMusicVolume;
            }

            if (_musicSource && !_musicSource.isPlaying) _musicSource.Play();

            GameManager.GetMonoSystem<ILightMonoSystem>().ToggleAudio(false);
        }

        private IEnumerator FadeOutMusic(AudioSource source, float duration)
        {
            float startVolume = source.volume;

            float time = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, 0f, time / duration);
                yield return null;
            }

            source.volume = 0f;
            source.Stop();
        }

        private void Play()
        {
            WFGameManager.HideCursor();

            if (_musicFadeRoutine != null) StopCoroutine(_musicFadeRoutine);
            _musicFadeRoutine = StartCoroutine(FadeOutMusic(_musicSource, 2f));

            _visualMS.FadeOut(2f).Then(_ =>
            {
                _musicSource.Stop();
                GameManager.GetMonoSystem<ILightMonoSystem>().ToggleAudio(true);
                _visualMS.FadeIn(5f);
                _mainMenuBackground.SetActive(false);
                _mainMenuScene.SetActive(false);
                _settingBackdrop.SetActive(true);
                PSXEffectSettings psxSettings = _visualMS.GetPSXSettings();
                psxSettings.EnableFog.value = false;
                _lightMS.SetColor(null, LightState.Emergency);
                _lightMS.SetIntensity(null, LightState.Emergency);
                WFGameManager.IsPaused = false;
                GameManager.GetMonoSystem<IGameLogicMonoSystem>().TriggerEvent("Start");
                GameManager.GetMonoSystem<IUIMonoSystem>().Show<GameView>();
            });
        }

        private void Settings()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<SettingsView>();
        }

        private void Quit()
        {
            WFGameManager.QuitGame();
        }

        public override void Show()
        {
            base.Show();
            WFGameManager.ShowCursor();
            WFGameManager.IsPaused = true;
            _mainMenuBackground.SetActive(true);
            _mainMenuScene.SetActive(true);
            _settingBackdrop.SetActive(false);

            PSXEffectSettings psxSettings = _visualMS.GetPSXSettings();
            psxSettings.EnableFog.value = true;
        }

        public override void Init()
        {
            _lightMS = GameManager.GetMonoSystem<ILightMonoSystem>();
            _visualMS = GameManager.GetMonoSystem<IVisualEffectMonoSystem>();
            //_lightMS.Toggle(LightState.Emergency);

            _play.onPointerDown.AddListener(Play);
            _settings.onPointerDown.AddListener(Settings);
            _quit.onPointerDown.AddListener(Quit);
        }
    }
}
