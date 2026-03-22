using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;
using WrongFloor.UI;

namespace WrongFloor
{
    public class PausedView : View
    {
        [SerializeField] private EventButton _resume;
        [SerializeField] private EventButton _settings;
        [SerializeField] private EventButton _quit;

        public void Resume()
        {
            WFGameManager.IsPaused = false;
            GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
        }

        private void Settings()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<SettingsView>();
        }

        private void Quit()
        {
            Application.Quit();
        }

        public override void Show()
        {
            base.Show();
            WFGameManager.ShowCursor();
            WFGameManager.IsPaused = true;
        }

        public override void Init()
        {
            _resume.onPointerDown.AddListener(Resume);
            _settings.onPointerDown.AddListener(Settings);
            _quit.onPointerDown.AddListener(Quit);
        }
    }
}
