using PlazmaGames.Core.MonoSystem;

namespace WrongFloor
{
    public interface IGameLogicMonoSystem : IMonoSystem
    {
        void TriggerEvent(string eventName);
        void Trigger(string triggerName);
        void SetInRange(string rangeName, bool state);
    }
}