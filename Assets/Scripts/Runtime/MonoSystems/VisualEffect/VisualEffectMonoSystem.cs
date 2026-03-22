using ColbyO.VNTG.PSX;
using UnityEngine;
using UnityEngine.Rendering;

namespace WrongFloor.MonoSystems
{
    public class VisualEffectMonoSystem : MonoBehaviour, IVisualEffectMonoSystem
    {
        private Volume _volume;

        private void Start()
        {
            _volume = FindAnyObjectByType<Volume>();
        }

        public PSXEffectSettings GetPSXSettings()
        {
            if (_volume == null) _volume = FindAnyObjectByType<Volume>();

            if (_volume && _volume.profile && _volume.profile.TryGet(out PSXEffectSettings psx)) return psx;

            return null;
        }
    }
}