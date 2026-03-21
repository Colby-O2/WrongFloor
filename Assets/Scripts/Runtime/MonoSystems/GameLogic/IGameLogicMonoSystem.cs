using PlazmaGames.Core.MonoSystem;

namespace WrongFloor
{
    public interface IGameLogicMonoSystem : IMonoSystem
    {
        void TriggerEvent(string eventName);
        void SetInRange(string rangeName, bool state);
    }
}