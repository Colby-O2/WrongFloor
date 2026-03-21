using PlazmaGames.Core;
using UnityEngine;
using WrongFloor.MonoSystems;

namespace WrongFloor
{
    public class Button : MonoBehaviour
    {
        [SerializeField] private string _name = "Button";

        void Start()
        {
            GameManager.GetMonoSystem<IInputMonoSystem>().InteractCallback.AddListener(Press);
        }

        void Update()
        {
            if (IsInRange()) return;
        }
        
        private void Press()
        {
            if (!IsInRange()) return;
            GameManager.GetMonoSystem<IGameLogicMonoSystem>().Trigger(_name);
        }
        
        private bool IsInRange()
        {
            return
                Vector3.Distance(WFGameManager.Player.transform.position, transform.position) <
                WFGameManager.Preferences.InteractionDistance;
        }
        
    }
}
