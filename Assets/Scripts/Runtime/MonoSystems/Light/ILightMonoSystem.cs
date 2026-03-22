using PlazmaGames.Core.MonoSystem;
using UnityEngine;
using WrongFloor.MonoSystems;

namespace WrongFloor
{
    public interface ILightMonoSystem : IMonoSystem
    {
        public void Subscribe(LightController lc, LightState state);
        public void Unsubscribe(LightController lc, LightState state);
        public void Toggle(LightState state);
        public void ToggleAudio(bool state);
        public void SetIntensity(float? val, LightState state);
        public void SetColor(Color? color, LightState state);
    }
}