using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;
using WrongFloor.MonoSystems;
using WrongFloor.UI;

namespace WrongFloor
{
    public class Button : MonoBehaviour
    {
        [SerializeField] private string _name = "Button";
        [SerializeField] private string _hint = "To Press";

        private bool _in = false;

        void Start()
        {
            GameManager.GetMonoSystem<IInputMonoSystem>().InteractCallback.AddListener(Press);
        }

        void Update()
        {
            bool now = IsInRange();
            if (_in && !now)
            {
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().SetHint("");
            }
            else if (!_in && now)
            {
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().SetHint("[E] " + _hint);
            }

            _in = now;
        }
        
        private void Press()
        {
            if (!_in) return;
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
