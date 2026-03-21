using DialogueGraph;
using WrongFloor.UI;
using PlazmaGames.Core;
using PlazmaGames.UI;

namespace WrongFloor.MonoSystems
{
    public class DialogueMonoSystem : DialogueController, IDialogueMonoSystem
    {
        private void StartDialogue(string dialogueName, System.Action finsihCallback, bool passive)
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().SetPassive(passive);
            base.StartDialogue(dialogueName, finsihCallback);
        }
        public override void StartDialogue(string dialogueName, System.Action finsihCallback = null)
        {
            StartDialogue(dialogueName, finsihCallback, false);
        }
        
        public Promise StartDialoguePromise(string dialogueName, bool passive = false)
        {
            Promise p = new Promise();
            StartDialogue(dialogueName, () => p.Resolve(), passive);
            return p;
        }
        
        public override void OnDialogueFinished()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().HideDialogue();
        }

        protected override void PlayDialogueNode()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().ShowDialogue();
            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().DisplayDialogue(_currentDialogueNode);
        }
    }
}
