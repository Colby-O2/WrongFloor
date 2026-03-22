using System;
using PlazmaGames.Core;
using UnityEngine;

namespace WrongFloor
{
    public class EventRange : MonoBehaviour
    {
        [SerializeField] private string _eventName;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.GetMonoSystem<IGameLogicMonoSystem>().SetInRange(_eventName, true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.GetMonoSystem<IGameLogicMonoSystem>().SetInRange(_eventName,false);
            }
        }
    }
}
