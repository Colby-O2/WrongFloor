using ColbyO.VNTG.PSX;
using PlazmaGames.Core;
using UnityEngine;
using UnityEngine.Rendering;

namespace WrongFloor.MonoSystems
{
    public class VisualEffectMonoSystem : MonoBehaviour, IVisualEffectMonoSystem
    {
        private Volume _volume;

        private ScreenFade _screenFade;

        private void Start()
        {
            _volume = FindAnyObjectByType<Volume>();
            _screenFade = FindAnyObjectByType<ScreenFade>();
        }

        public Promise FadeIn(float duration)
        {
            return _screenFade.FadeIn(duration);
        }

        public Promise FadeOut(float duration)
        {
            return _screenFade.FadeOut(duration);
        }

        public PSXEffectSettings GetPSXSettings()
        {
            if (_volume == null) _volume = FindAnyObjectByType<Volume>();

            if (_volume && _volume.profile && _volume.profile.TryGet(out PSXEffectSettings psx)) return psx;

            return null;
        }
    }
}