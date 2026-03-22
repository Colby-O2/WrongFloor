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
        private HashSet<string> _inRange = new();
        private HashSet<string> _triggers = new();
        private bool _started = false;

        private IDialogueMonoSystem _dialogueMs;

        private static class Refs
        {
            public static Elevator Elevator;
            public static Button ControlPanel;
            public static Button ElevatorPanel;
            public static EventRange ElevatorDoor;
            public static SoundScapeManager SoundScape;
            public static LightFlicker LightFlicker1;
            public static LightFlicker LightFlicker2;
            public static LightFlicker LightFlicker3;
            public static LightFlicker LightFlicker4;
            public static GameObject Jumpscare;
            public static AudioSource Crying1;
            public static AudioSource Crying2;
            public static PanelController ControlPanelController;
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
            Refs.Elevator = GameObject.FindAnyObjectByType<Elevator>();
            Refs.ElevatorPanel = GameObject.FindGameObjectWithTag("ElevatorPanel").GetComponent<Button>();
            Refs.ElevatorDoor = GameObject.FindGameObjectWithTag("ElevatorDoor").GetComponent<EventRange>();

            Refs.ControlPanel = GameObject.FindGameObjectWithTag("ControlPanel").GetComponent<Button>();
            Refs.ControlPanelController = Refs.ControlPanel.GetComponent<PanelController>();

            Refs.SoundScape = FindAnyObjectByType<SoundScapeManager>();

            Refs.LightFlicker1 = GameObject.FindGameObjectWithTag("LightFlicker1").GetComponent<LightFlicker>();
            Refs.LightFlicker2 = GameObject.FindGameObjectWithTag("LightFlicker2").GetComponent<LightFlicker>();
            Refs.LightFlicker3 = GameObject.FindGameObjectWithTag("LightFlicker3").GetComponent<LightFlicker>();
            Refs.LightFlicker4 = GameObject.FindGameObjectWithTag("LightFlicker4").GetComponent<LightFlicker>();

            Refs.Jumpscare = GameObject.FindGameObjectWithTag("Jumpscare");
            Refs.Jumpscare.SetActive(false);

            Refs.Crying1 = GameObject.FindGameObjectWithTag("Crying1").GetComponent<AudioSource>();
            Refs.Crying2 = GameObject.FindGameObjectWithTag("Crying2").GetComponent<AudioSource>();

            Refs.Crying1.Stop();
            Refs.Crying2.Stop();
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

                    Refs.ElevatorPanel.Disable();
                    Refs.ControlPanel.Disable();
                    Refs.ElevatorDoor.Enabled = false;

                    Refs.SoundScape.WindVolume(0.25f);
                    Refs.SoundScape.HeartVolume(0f, 0f);
                    Refs.SoundScape.AmbienceVolume(0f, 0f);

                    GameManager.GetMonoSystem<ILightMonoSystem>().Toggle(LightState.Nomral);

                    Refs.Elevator.MoveDown().Then(_ =>
                    {
                        Refs.ElevatorPanel.Enable();
                        Refs.ElevatorPanel.UpdateHint("Call For Help");

                        GameManager.GetMonoSystem<ILightMonoSystem>().Toggle(LightState.Emergency);
                        GameManager.GetMonoSystem<ILightMonoSystem>().SetIntensity(3.0f, LightState.Emergency);

                        // Loop 1
                        _scheduler.When(() => IsTriggered("Button"))
                        .Then(_ => _dialogueMs.StartDialoguePromise("Operator", passive: true))
                        .Then(_ => Refs.Elevator.OpenDoors())
                        .Then(_ =>
                        {
                            Refs.ElevatorDoor.Enabled = true;
                            Refs.ControlPanel.Enable();
                        })
                        .Then(_ => _scheduler.When(() => IsTriggered("Fix")))
                        .Then(_ =>
                        {
                            GameManager.GetMonoSystem<ILightMonoSystem>().Toggle(LightState.Nomral);
                            Refs.ElevatorPanel.UpdateHint("Go To Lobby");
                            Refs.ElevatorPanel.Enable();
                            Refs.Elevator.MoveToCorrectPosition();
                        })
                        .Then(_ => _scheduler.When(() => IsTriggered("Button")))
                        .Then(_ =>
                        {
                            Refs.ElevatorDoor.Enabled = false;
                        })
                        .Then(_ => Refs.Elevator.CloseDoors())
                        .Then(_ => Refs.Elevator.MoveDown())
                        .Then(_ =>
                        {
                            GameManager.GetMonoSystem<ILightMonoSystem>().Toggle(LightState.Emergency);
                            GameManager.GetMonoSystem<ILightMonoSystem>().SetIntensity(2f, LightState.Emergency);

                            Refs.Elevator.MoveToWrongPosition(true);
                            Refs.ElevatorPanel.Enable();
                            Refs.ElevatorPanel.UpdateHint("Call For Help");
                        })
                        // Loop 2
                        .Then(_ => _scheduler.When(() => IsTriggered("Button")))
                        .Then(_ => _dialogueMs.StartDialoguePromise("Operator", passive: true))
                        .Then(_ => Refs.Elevator.OpenDoors())
                        .Then(_ =>
                        {
                            Refs.SoundScape.WindVolume(0.5f);
                            Refs.ElevatorDoor.Enabled = true;
                            Refs.ControlPanel.Enable();
                        })
                        .Then(_ => _scheduler.When(() => IsTriggered("Fix")))
                        .Then(_ =>
                        {
                            GameManager.GetMonoSystem<ILightMonoSystem>().Toggle(LightState.Nomral);
                            Refs.LightFlicker3.Disable = false;
                            Refs.ElevatorPanel.UpdateHint("Go To Lobby");
                            Refs.ElevatorPanel.Enable();
                            Refs.Elevator.MoveToCorrectPosition();
                        })
                        .Then(_ => _scheduler.When(() => IsTriggered("Button")))
                        .Then(_ =>
                        {
                            Refs.ElevatorDoor.Enabled = false;
                        })
                        .Then(_ => Refs.Elevator.CloseDoors())
                        .Then(_ => Refs.Elevator.MoveDown())
                        .Then(_ =>
                        {
                            GameManager.GetMonoSystem<ILightMonoSystem>().Toggle(LightState.Emergency);
                            GameManager.GetMonoSystem<ILightMonoSystem>().SetIntensity(1f, LightState.Emergency);

                            Refs.Elevator.MoveToWrongPosition(true);
                            Refs.ElevatorPanel.Enable();
                            Refs.ElevatorPanel.UpdateHint("Call For Help");
                        })
                        // Loop 3
                        .Then(_ => _scheduler.When(() => IsTriggered("Button")))
                        .Then(_ => _dialogueMs.StartDialoguePromise("Operator", passive: true))
                        .Then(_ => Refs.Elevator.OpenDoors())
                        .Then(_ =>
                        {
                            Refs.SoundScape.WindVolume(1f);
                            Refs.ElevatorDoor.Enabled = true;
                            Refs.ControlPanel.Enable();
                        })
                        .Then(_ => _scheduler.When(() => IsTriggered("Fix")))
                        .Then(_ =>
                        {
                            Refs.ControlPanelController.PlayWarning();
                            Refs.SoundScape.StopWind();
                            GameManager.GetMonoSystem<ILightMonoSystem>().Toggle(LightState.Nomral);
                            Refs.LightFlicker2.Disable = false;
                            Refs.ElevatorPanel.UpdateHint("Go To Lobby");
                            Refs.ElevatorPanel.Enable();
                            Refs.Elevator.MoveToCorrectPosition();
                            Refs.Crying1.Play();
                        })
                        .Then(_ => _scheduler.When(() => IsTriggered("Button")))
                        .Then(_ =>
                        {
                            Refs.ElevatorDoor.Enabled = false;
                        })
                        .Then(_ => Refs.Elevator.CloseDoors())
                        .Then(_ => Refs.Elevator.MoveDown())
                        .Then(_ =>
                        {
                            GameManager.GetMonoSystem<ILightMonoSystem>().Toggle(LightState.Emergency);
                            GameManager.GetMonoSystem<ILightMonoSystem>().SetColor(Color.red, LightState.Emergency);
                            GameManager.GetMonoSystem<ILightMonoSystem>().SetIntensity(0.5f, LightState.Emergency);

                            Refs.Crying2.Play();

                            Refs.Elevator.MoveToWrongPosition(true);
                            Refs.ElevatorPanel.Enable();
                            Refs.ElevatorPanel.UpdateHint("Call For Help");
                        })
                        // Loop 4
                        .Then(_ => _scheduler.When(() => IsTriggered("Button")))
                        .Then(_ => _dialogueMs.StartDialoguePromise("Operator", passive: true))
                        .Then(_ => Refs.Elevator.OpenDoors())
                        .Then(_ =>
                        {
                            Refs.ElevatorDoor.Enabled = true;
                            Refs.ControlPanel.Enable();
                            Refs.Jumpscare.SetActive(true);
                        })
                        .Then(_ => _scheduler.When(() => IsInRange("Jumpscare")))
                        .Then(_ => 
                        {
                            Refs.Crying1.Stop();
                            Refs.Crying2.Stop();
                            Refs.SoundScape.PlayJumpScare();
                            Refs.Jumpscare.SetActive(false);
                            Refs.SoundScape.HeartVolume(0.25f);
                            Refs.SoundScape.PlayHeart();

                        })
                        .Then(_ => _scheduler.When(() => IsTriggered("Fix")))
                        .Then(_ =>
                        {
                            Refs.SoundScape.AmbienceVolume(0.25f);
                            Refs.SoundScape.PlayAmbience();
                            GameManager.GetMonoSystem<ILightMonoSystem>().Toggle(LightState.Nomral);
                            Refs.LightFlicker4.Disable = false;
                            Refs.LightFlicker1.Disable = false;
                            Refs.ElevatorPanel.UpdateHint("Go To Lobby");
                            Refs.ElevatorPanel.Enable();
                            Refs.Elevator.MoveToCorrectPosition();
                        })
                        .Then(_ => _scheduler.When(() => IsTriggered("Button")))
                        .Then(_ =>
                        {
                            Refs.ElevatorDoor.Enabled = false;
                        })
                        .Then(_ => Refs.Elevator.CloseDoors())
                        .Then(_ => Refs.Elevator.MoveDown())
                        .Then(_ =>
                        {
                            GameManager.GetMonoSystem<ILightMonoSystem>().Toggle(LightState.Emergency);
                            GameManager.GetMonoSystem<ILightMonoSystem>().SetIntensity(1f, LightState.Emergency);

                            Refs.Elevator.MoveToWrongPosition(true);
                            Refs.ElevatorPanel.Enable();
                            Refs.ElevatorPanel.UpdateHint("Call For Help");
                        })
                        // Loop 5
                        .Then(_ => _scheduler.When(() => IsTriggered("Button")))
                        .Then(_ => { Refs.ElevatorPanel.Disable(); })
                        .Then(_ => _dialogueMs.StartDialoguePromise("Operator", passive: true))
                        .Then(_ =>
                        {
                            Refs.SoundScape.AmbienceVolume(0.5f);
                            Refs.SoundScape.HeartVolume(0.5f);
                            Refs.ElevatorDoor.Enabled = true;
                            Refs.ControlPanel.Enable();
                        })
                        .Then(_ => Refs.Elevator.OpenDoors())
                        .Then(_ => _scheduler.When(() => IsTriggered("Fix")))
                        .Then(_ =>
                        {
                            Refs.ControlPanelController.PlayCrash();
                            GameManager.GetMonoSystem<ILightMonoSystem>().Toggle(LightState.Off);
                            Refs.ElevatorPanel.UpdateHint("Go To Lobby");
                            Refs.ElevatorPanel.Enable();
                            Refs.Elevator.MoveToCorrectPosition();
                        })
                        .Then(_ => _scheduler.When(() => IsTriggered("Button")))
                        .Then(_ =>
                        {
                            Refs.ElevatorDoor.Enabled = false;
                        })
                        .Then(_ => Refs.Elevator.CloseDoors())
                        .Then(_ => Refs.Elevator.MoveDown())
                        .Then(_ => 
                        { 
                            Refs.ElevatorPanel.Enable();
                            Refs.ElevatorPanel.UpdateHint("Call For Help");
                        })
                        .Then(_ => _scheduler.When(() => IsTriggered("Button")))
                        .Then(_ =>
                        {
                            Refs.ElevatorPanel.Disable();
                            Refs.Elevator.MoveToWrongPosition(true);
                        })
                        .Then(_ =>
                        {
                            Refs.SoundScape.StopAmbience();
                            Refs.SoundScape.StopHeart();
                        })
                        .Then(_ => _dialogueMs.StartDialoguePromise("Operator", passive: true))
                        .Then(_ => _scheduler.Wait(Random.Range(2f, 7f)))
                        .Then(_ => Refs.Elevator.FallElevator())
                        .Then(_ => _scheduler.Wait(5f))
                        .Then(_ =>
                        {
                            GameManager.GetMonoSystem<IUIMonoSystem>().Show<MainMenuView>();
                            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<MainMenuView>().HasBeatGame = true;
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