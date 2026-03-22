using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;
using WrongFloor.MonoSystems;
using WrongFloor.UI;
using static UnityEngine.Rendering.DebugUI.Table;

namespace WrongFloor
{
    public class Button : MonoBehaviour
    {
        [SerializeField] private AudioSource _as;
        [SerializeField] private AudioClip _clip;

        [SerializeField] private string _name = "Button";
        [SerializeField] private string _hint = "To Press";

        private bool _in = false;

        private bool _disabled = false;

        private void Start()
        {
            GameManager.GetMonoSystem<IInputMonoSystem>().InteractCallback.AddListener(Press);
        }

        private void Update()
        {
            bool now = IsInRange();
            if (_in && !now)
            {
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().SetHint("");
            }
            else if (!_in && now && !_disabled)
            {
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().SetHint("[E] " + _hint);
            }

            _in = now;
        }
        
        public void Enable()
        {
            _disabled = false;
            if (IsInRange())
            {
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().SetHint("[E] " + _hint);
            }
        }

        public void Disable()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().SetHint("");
            _disabled = true;
        }

        public void UpdateHint(string hint)
        {
            _hint = hint;
            if (!_in && IsInRange() && !_disabled)
            {
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().SetHint("[E] " + _hint);
            }
        }

        private void Press()
        {
            if (!_in || _disabled) return;
            Disable();
            if (_as && _clip) _as.PlayOneShot(_clip);
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
