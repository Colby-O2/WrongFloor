using ColbyO.VNTG.PSX;
using PlazmaGames.Core.MonoSystem;

namespace WrongFloor.MonoSystems
{
    public interface IVisualEffectMonoSystem : IMonoSystem
    {
        public PSXEffectSettings GetPSXSettings();
    }
}