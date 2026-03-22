using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using WrongFloor.Player;
using WrongFloor.UI;

namespace WrongFloor
{
    public class SettingsView : View
    {
        [SerializeField] private MovementSettings _movementSettings;

        [SerializeField] private Slider _volume;
        [SerializeField] private Slider _sensitivity;

        [SerializeField] private EventButton _back;

        private void Back()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
        }

        private float GetSensitivityAdjustedValue(float input, float exp = 2f)
        {
            return Mathf.Pow(input, exp);
        }

        private void OnVolumeChanged(float vol)
        {
            GameManager.GetMonoSystem<IAudioMonoSystem>().SetOverallVolume(vol);
        }

        private void OnSensitivityChanged(float val)
        {
            float sens = Mathf.Lerp(0.01f, 1f, GetSensitivityAdjustedValue(val));
            _movementSettings.Sensitivity = sens;
        }

        public override void Init()
        {
            _volume.onValueChanged.AddListener(OnVolumeChanged);
            _sensitivity.onValueChanged.AddListener(OnSensitivityChanged);
            _back.onPointerDown.AddListener(Back);

            _volume.value = GameManager.GetMonoSystem<IAudioMonoSystem>().GetOverallVolume();
            _sensitivity.value = 0.5f;
        }

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Back();
            }
        }
    }
}