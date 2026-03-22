using PlazmaGames.Core;
using PlazmaGames.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WrongFloor.MonoSystems;

namespace WrongFloor
{
    public class GameLogicMonoSystem : MonoBehaviour, IGameLogicMonoSystem
    {
        private Scheduler _scheduler = new();
        private Refs _refs = new Refs();
        private HashSet<string> _inRange = new();
        private HashSet<string> _triggers = new();
        private bool _started = false;

        private IDialogueMonoSystem _dialogueMs;

        private class Refs
        {
            public Elevator Elevator;
            public Button ControlPanel;
            public Button ElevatorPanel;
            public EventRange ElevatorDoor;
            public SoundScapeManager SoundScape;
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoad;
            SceneManager.sceneUnloaded += OnSceneUnload;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoad;
            SceneManager.sceneUnloaded -= OnSceneUnload;
        }

        private void Start()
        {
            _dialogueMs = GameManager.GetMonoSystem<IDialogueMonoSystem>();
        }

        private void Update()
        {
            _scheduler.Tick(Time.deltaTime);
        }

        private void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            _refs.Elevator = GameObject.FindAnyObjectByType<Elevator>();
            _refs.ControlPanel = GameObject.FindGameObjectWithTag("ControlPanel").GetComponent<Button>();
            _refs.ElevatorPanel = GameObject.FindGameObjectWithTag("ElevatorPanel").GetComponent<Button>();
            _refs.ElevatorDoor = GameObject.FindGameObjectWithTag("ElevatorDoor").GetComponent<EventRange>();
            _refs.SoundScape = FindAnyObjectByType<SoundScapeManager>();
        }

        private void OnSceneUnload(Scene scene)
        {

        }

        public void TriggerEvent(string eventName)
        {
            Debug.Log("Event Triggered: " + eventName);
            switch (eventName)
            {
                case "Start":

                    _refs.ElevatorPanel.Disable();
                    _refs.ControlPanel.Disable();
                    _refs.ElevatorDoor.Enabled = false;

                    _refs.SoundScape.WindVolume(0.25f);

                    GameManager.GetMonoSystem<ILightMonoSystem>().Toggle(LightState.Emergency);

                    _refs.Elevator.MoveDown().Then(_ =>
                    {
                        _refs.ElevatorPanel.Enable();
                        _refs.ElevatorPanel.UpdateHint("Call For Help");
                        // Loop 1
                        _scheduler.When(() => IsTriggered("Button"))
                        .Then(_ => _dialogueMs.StartDialoguePromise("Operator"))
                        .Then(_ => _refs.Elevator.OpenDoors())
                        .Then(_ =>
                        {
                            _refs.ElevatorDoor.Enabled = true;
                            _refs.ControlPanel.Enable();
                        })
                        .Then(_ => _scheduler.When(() => IsTriggered("Fix")))
                        .Then(_ =>
                        {
                            GameManager.GetMonoSystem<ILightMonoSystem>().Toggle(LightState.Nomral);
                            _refs.ElevatorPanel.UpdateHint("Go To Lobby");
                            _refs.ElevatorPanel.Enable();
                            _refs.Elevator.MoveToCorrectPosition();
                        })
                        .Then(_ => _scheduler.When(() => IsTriggered("Button")))
                        .Then(_ =>
                        {
                            _refs.ElevatorDoor.Enabled = false;
                        })
                        .Then(_ => _refs.Elevator.CloseDoors())
                        .Then(_ =>
                        {
                            GameManager.GetMonoSystem<ILightMonoSystem>().Toggle(LightState.Emergency);
                        })
                        .Then(_ => _refs.Elevator.MoveDown())
                        .Then(_ =>
                        {
                            _refs.Elevator.MoveToWrongPosition(true);
                            _refs.ElevatorPanel.Enable();
                            _refs.ElevatorPanel.UpdateHint("Call For Help");
                        })
                        // Loop 2
                        .Then(_ => _scheduler.When(() => IsTriggered("Button")))
                        .Then(_ => _dialogueMs.StartDialoguePromise("Operator"))
                        .Then(_ => _refs.Elevator.OpenDoors())
                        .Then(_ =>
                        {
                            _refs.SoundScape.WindVolume(0.5f);
                            _refs.ElevatorDoor.Enabled = true;
                            _refs.ControlPanel.Enable();
                        })
                        .Then(_ => _scheduler.When(() => IsTriggered("Fix")))
                        .Then(_ =>
                        {
                            GameManager.GetMonoSystem<ILightMonoSystem>().Toggle(LightState.Nomral);
                            _refs.ElevatorPanel.UpdateHint("Go To Lobby");
                            _refs.ElevatorPanel.Enable();
                            _refs.Elevator.MoveToCorrectPosition();
                        })
                        .Then(_ => _scheduler.When(() => IsTriggered("Button")))
                        .Then(_ =>
                        {
                            _refs.ElevatorDoor.Enabled = false;
                        })
                        .Then(_ => _refs.Elevator.CloseDoors())
                        .Then(_ =>
                        {
                            GameManager.GetMonoSystem<ILightMonoSystem>().Toggle(LightState.Emergency);
                        })
                        .Then(_ => _refs.Elevator.MoveDown())
                        .Then(_ =>
                        {
                            _refs.Elevator.MoveToWrongPosition(true);
                            _refs.ElevatorPanel.Enable();
                            _refs.ElevatorPanel.UpdateHint("Call For Help");
                        })
                        // Loop 3
                        .Then(_ => _scheduler.When(() => IsTriggered("Button")))
                        .Then(_ => _dialogueMs.StartDialoguePromise("Operator"))
                        .Then(_ => _refs.Elevator.OpenDoors())
                        .Then(_ =>
                        {
                            _refs.SoundScape.WindVolume(1f);
                            _refs.ElevatorDoor.Enabled = true;
                            _refs.ControlPanel.Enable();
                        })
                        .Then(_ => _scheduler.When(() => IsTriggered("Fix")))
                        .Then(_ =>
                        {
                            _refs.SoundScape.StopWind();
                            GameManager.GetMonoSystem<ILightMonoSystem>().Toggle(LightState.Nomral);
                            _refs.ElevatorPanel.UpdateHint("Go To Lobby");
                            _refs.ElevatorPanel.Enable();
                            _refs.Elevator.MoveToCorrectPosition();
                        })
                        .Then(_ => _scheduler.When(() => IsTriggered("Button")))
                        .Then(_ =>
                        {
                            _refs.ElevatorDoor.Enabled = false;
                        })
                        .Then(_ => _refs.Elevator.CloseDoors())
                        .Then(_ =>
                        {
                            GameManager.GetMonoSystem<ILightMonoSystem>().Toggle(LightState.Emergency);
                        })
                        .Then(_ => _refs.Elevator.MoveDown())
                        .Then(_ =>
                        {
                            _refs.Elevator.MoveToWrongPosition(true);
                            _refs.ElevatorPanel.Enable();
                            _refs.ElevatorPanel.UpdateHint("Call For Help");
                        })
                        // Loop 4
                        .Then(_ => _scheduler.When(() => IsTriggered("Button")))
                        .Then(_ => _dialogueMs.StartDialoguePromise("Operator"))
                        .Then(_ => _refs.Elevator.OpenDoors())
                        .Then(_ =>
                        {
                            _refs.ElevatorDoor.Enabled = true;
                            _refs.ControlPanel.Enable();
                        })
                        .Then(_ => _scheduler.When(() => IsTriggered("Fix")))
                        .Then(_ =>
                        {
                            GameManager.GetMonoSystem<ILightMonoSystem>().Toggle(LightState.Nomral);
                            _refs.ElevatorPanel.UpdateHint("Go To Lobby");
                            _refs.ElevatorPanel.Enable();
                            _refs.Elevator.MoveToCorrectPosition();
                        })
                        .Then(_ => _scheduler.When(() => IsTriggered("Button")))
                        .Then(_ =>
                        {
                            _refs.ElevatorDoor.Enabled = false;
                        })
                        .Then(_ => _refs.Elevator.CloseDoors())
                        .Then(_ =>
                        {
                            GameManager.GetMonoSystem<ILightMonoSystem>().Toggle(LightState.Emergency);
                        })
                        .Then(_ => _refs.Elevator.MoveDown())
                        .Then(_ =>
                        {
                            _refs.Elevator.MoveToWrongPosition(true);
                            _refs.ElevatorPanel.Enable();
                            _refs.ElevatorPanel.UpdateHint("Call For Help");
                        })
                        // Loop 5
                        .Then(_ => _scheduler.When(() => IsTriggered("Button")))
                        .Then(_ => _dialogueMs.StartDialoguePromise("Operator"))
                        .Then(_ => _refs.Elevator.OpenDoors())
                        .Then(_ =>
                        {
                            _refs.ElevatorDoor.Enabled = true;
                            _refs.ControlPanel.Enable();
                        })
                        .Then(_ => _scheduler.When(() => IsTriggered("Fix")))
                        .Then(_ =>
                        {
                            GameManager.GetMonoSystem<ILightMonoSystem>().Toggle(LightState.Nomral);
                            _refs.ElevatorPanel.UpdateHint("Go To Lobby");
                            _refs.ElevatorPanel.Enable();
                            _refs.Elevator.MoveToCorrectPosition();
                        })
                        .Then(_ => _scheduler.When(() => IsTriggered("Button")))
                        .Then(_ =>
                        {
                            _refs.ElevatorDoor.Enabled = false;
                        })
                        .Then(_ => _refs.Elevator.CloseDoors())
                        .Then(_ => _refs.Elevator.MoveDown())
                        .Then(_ =>
                        {
                            _refs.Elevator.MoveToWrongPosition(true);
                            _refs.ElevatorPanel.Enable();
                            _refs.ElevatorPanel.UpdateHint("Call For Help");
                        })
                        .Then(_ => _scheduler.When(() => IsTriggered("Button")))
                        .Then(_ => _dialogueMs.StartDialoguePromise("Operator"))
                        .Then(_ => _refs.Elevator.OpenDoors(false))
                        .Then(_ => _refs.Elevator.FallElevator())
                        .Then(_ => _scheduler.Wait(3f))
                        .Then(_ =>
                        {
                            GameManager.GetMonoSystem<IUIMonoSystem>().Show<MainMenuView>();
                            GameManager.GetMonoSystem<IVisualEffectMonoSystem>().FadeIn(0f);
                            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                        });
                    });
                    break;
                
                default:
                    Debug.LogError("Unhandled GameLogic event:" + eventName);
                    break;
            }
        }

        public void Trigger(string triggerName)
        {
            _triggers.Add(triggerName);
        }
        
        public void SetInRange(string rangeName, bool state)
        {
            if (state) _inRange.Add(rangeName);
            else _inRange.Remove(rangeName);

            switch (rangeName)
            {
                case "Crouch":
                    Debug.Log("Crouch " + state.ToString());
                    if (state) WFGameManager.Player.Crouch();
                    else WFGameManager.Player.Uncrouch();
                    break;
                default: break;
            }
        }

        private bool IsTriggered(string triggerName) => _triggers.Remove(triggerName);

        private bool IsInRange(string rangeName) => _inRange.Contains(rangeName);
    }
}