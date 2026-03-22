using System;
using System.Collections.Generic;
using PlazmaGames.Core;
using UnityEngine;
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
            public Elevator elevator;
        }
        
        private void Start()
        {
            _dialogueMs = GameManager.GetMonoSystem<IDialogueMonoSystem>();
            _refs.elevator = GameObject.FindAnyObjectByType<Elevator>();
        }

        private void Update()
        {
            if (!_started)
            {
                _started = true;
                TriggerEvent("Start");
            }
            _scheduler.Tick(Time.deltaTime);
        }
        
        
        public void TriggerEvent(string eventName)
        {
            Debug.Log("Event Triggered: " + eventName);
            switch (eventName)
            {
                case "Start":
                    _scheduler.When(() => IsTriggered("Button"))
                        .Then(_ => _dialogueMs.StartDialoguePromise("Test"))
                        .Then(_ => _refs.elevator.OpenDoors())
                        .Then(_ =>
                        {
                            Debug.Log("WOW :)");
                        })
                        ;
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