using System.Collections.Generic;
using PlazmaGames.Core;
using UnityEngine;

namespace WrongFloor
{
    public class GameLogicMonoSystem : MonoBehaviour, IGameLogicMonoSystem
    {
        private Scheduler _scheduler = new();
        private Refs _refs = new Refs();
        private HashSet<string> _inRange = new();

        private class Refs
        {
        }
        
        private void Start()
        {
            
        }

        private void Update()
        {
            _scheduler.Tick(Time.deltaTime);
        }
        
        public void TriggerEvent(string eventName)
        {
            Debug.Log("Event Triggered: " + eventName);
            switch (eventName)
            {
                case "Begin":
                    break;
                
                default:
                    Debug.LogError("Unhandled GameLogic event:" + eventName);
                    break;
            }
        }

        public void SetInRange(string rangeName, bool state)
        {
            if (state) _inRange.Add(rangeName);
            else _inRange.Remove(rangeName);
        }
    }
}