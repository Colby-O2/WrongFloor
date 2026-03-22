using DialogueGraph.Data;
using WrongFloor.MonoSystems;
using PlazmaGames.Attribute;
using PlazmaGames.Core;
using PlazmaGames.UI;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace WrongFloor.UI
{
    public class GameView : View
    {
        [SerializeField] private AudioSource _as;
        [SerializeField] private AudioSource _staticAS;
        [SerializeField] private GameObject _holder;

        [Header("Hint")]
        [SerializeField] private TMP_Text _hintText;

        [Header("Dialogue")]
        [SerializeField] private GameObject _dialogueHolder;
        [SerializeField] private TMP_Text _dialogueAvatarName;
        [SerializeField] private TMP_Text _dialogue;
        [SerializeField] private GameObject _dialogueHint;
        [SerializeField] private InputAction _dialogueNextAction;
        [SerializeField] private float _typeSpeed = 0.1f;
        [SerializeField] private float _timeout = 4f;
        [SerializeField] private float _passiveTimeout = 3f;
        [SerializeField, ReadOnly] private bool _passive = false;
        [SerializeField, ReadOnly] private float _timeSinceMessageEnd = 0;

        [SerializeField, ReadOnly] private DialogueNodeData _currentDialogueNode;
        [SerializeField, ReadOnly] private float _timeSinceWriteStart = 0f;
        [SerializeField, ReadOnly] private string _currentMessage;
        [SerializeField, ReadOnly] private bool _showedMessage = false;
        [SerializeField, ReadOnly] private bool _isTyping = false;
        [SerializeField, ReadOnly] private bool _showingChoice = false;
        [SerializeField, ReadOnly] private int _selectedChoice = -1;
        [SerializeField, ReadOnly] private bool _startedDialgoueAudio;

        private bool _initialInputLockState = false;

        private Coroutine _writeRoutine = null;

        private bool IsWriting() => _writeRoutine == null;

        private void StopWriteRoutine()
        {
            if (_writeRoutine == null) return;
            StopCoroutine(_writeRoutine);
            _writeRoutine = null;
        }

        public void SetHint(string text) => _hintText.text = text;

        public void SetPassive(bool state) => _passive = state;

        IEnumerator FadeOutAndStopAudio(float duration = 0.1f)
        {
            _startedDialgoueAudio = false;
            float startVolume = _as.volume;

            float t = 0f;
            while (t < duration)
            {
                if (_startedDialgoueAudio) break;
                t += Time.deltaTime;
                _as.volume = Mathf.Lerp(startVolume, 0f, t / duration);
                yield return null;
            }

            if (!_startedDialgoueAudio) _as.Stop();
            _as.volume = startVolume;
        }

        private void StartAudio()
        {
            _startedDialgoueAudio = true;
            if (!_as.isPlaying) _as.Play();
        }

        private IEnumerator TypeDialogue(
            string msg, 
            float typeSpeed, 
            TMP_Text target, 
            UnityAction onFinished = null, 
            bool isDialogue = true
        )
        {
            if (isDialogue)
            {
                _timeSinceMessageEnd = 0;
                _currentMessage = msg;
                _showedMessage = false;
                _timeSinceWriteStart = 0f;
            }

            if (_as && isDialogue) StartAudio();

            target.text = string.Empty;

            if (!isDialogue) _isTyping = true;

            for (int i = 0; i < msg.Length; i++)
            {
                while (WFGameManager.IsPaused)
                {
                    if (_as && isDialogue) StartCoroutine(FadeOutAndStopAudio());
                    yield return null;
                }

                if (_as && !_as.isPlaying && isDialogue) StartAudio();

                if (msg[i] == '<')
                {
                    int endIndex = msg.IndexOf('>', i);
                    if (endIndex != -1)
                    {
                        string fullTag = msg.Substring(i, endIndex - i + 1);
                        target.text += fullTag;
                        i = endIndex;
                        continue;
                    }
                }

                target.text += msg[i];
                yield return new WaitForSeconds(typeSpeed * WFGameManager.Preferences.DialogueSpeedMul);
            }

            target.text = msg;
            if (!isDialogue) _isTyping = false;
            if (_as && isDialogue) StartCoroutine(FadeOutAndStopAudio());

            if (isDialogue) _showedMessage = true;
            if (isDialogue && !_passive) _dialogueHint.SetActive(true);
        }

        public bool IsShowingDialogue()
        {
            return _currentDialogueNode != null;
        }

        public bool IsDialogueWaiting()
        {
            return IsShowingDialogue() && _showedMessage;
        }

        public void ShowDialogue()
        {
            if (!_dialogueHolder.activeSelf)
            {
                _initialInputLockState = WFGameManager.LockMovement;
            }
            _dialogueHolder.SetActive(true);
            _dialogueHint.SetActive(false);

            if (!_staticAS.isPlaying) _staticAS.Play();

            if (!_passive) WFGameManager.LockMovement = true;
            _hintText.gameObject.SetActive(false);
        }

        public void DisplayDialogue(DialogueNodeData dialogue)
        {
            StopWriteRoutine();
            _dialogueHint.SetActive(false);
            _currentDialogueNode = dialogue;
            if (_dialogueAvatarName) _dialogueAvatarName.text = dialogue.ActorName;
            _writeRoutine = StartCoroutine(TypeDialogue(
                dialogue.Text,
                 _typeSpeed,
                _dialogue,
                Next,
                true)
            );
        }

        public void HideDialogue()
        {
            if (_dialogueAvatarName) _dialogueAvatarName.text = string.Empty;
            _dialogue.text = string.Empty;
            _dialogueHolder.SetActive(false);
            _dialogueHint.SetActive(false);

            _staticAS.Stop();

            if (!_passive) WFGameManager.LockMovement = _initialInputLockState;
            _hintText.gameObject.SetActive(true);
        }

        public void GoToNext(int choice)
        {
            StopWriteRoutine();
            _showedMessage = false;
            GameManager.GetMonoSystem<IDialogueMonoSystem>().OnDialogueNodeFinish(choice);
        }

        private void Next()
        {
            if (_currentDialogueNode.Type == DialogueGraph.Enumeration.DialogueType.SingleChoice || _currentDialogueNode.Type == DialogueGraph.Enumeration.DialogueType.MultipleChoice)
            {
                GoToNext(0);
            }
        }

        private void HandleTimeout()
        {
            if (WFGameManager.IsPaused) return;

            _timeSinceWriteStart += Time.deltaTime;

            if (
                (_timeSinceWriteStart > _timeout && !_showedMessage) ||
                (!_passive && _dialogueNextAction.WasPressedThisFrame() && !_showedMessage))
            {
                StopWriteRoutine();
                _as?.Stop();
                _showedMessage = true;
                _dialogue.text = _currentMessage;
                if (!_passive) _dialogueHint.SetActive(true);
            }
            else if (_showedMessage)
            {
                if (_passive)
                {
                    _timeSinceMessageEnd += Time.deltaTime;

                    if (_timeSinceMessageEnd >= _passiveTimeout)
                    {
                        _timeSinceMessageEnd = -1;
                        StopWriteRoutine();
                        Next();
                    }
                }
                else if (_dialogueNextAction.WasPressedThisFrame())
                {
                    StopWriteRoutine();
                    Next();
                }
            }
        }

        public bool IsShowingChoice()
        {
            return _showingChoice;
        }

        public override void Show()
        {
            base.Show();
            _holder.gameObject.SetActive(true);
            _dialogueHint.SetActive(true);
            if (!IsShowingChoice()) WFGameManager.HideCursor();
            _dialogueNextAction.Enable();
        }

        public override void Hide()
        {
            _holder.gameObject.SetActive(false);

            _dialogueNextAction.Disable();
        }

        public override void Init()
        {
            HideDialogue();
        }

        private void Update()
        {
            if (WFGameManager.IsPaused) return;

            HandleTimeout();
        }
    }
}