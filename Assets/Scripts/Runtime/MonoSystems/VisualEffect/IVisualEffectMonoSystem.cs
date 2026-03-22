using ColbyO.VNTG.PSX;
using PlazmaGames.Core;
using PlazmaGames.Core.MonoSystem;

namespace WrongFloor.MonoSystems
{
    public interface IVisualEffectMonoSystem : IMonoSystem
    {
        public PSXEffectSettings GetPSXSettings();

        public Promise FadeIn(float duration);
        public Promise FadeOut(float duration);
    }
}