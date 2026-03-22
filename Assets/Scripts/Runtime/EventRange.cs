using System;
using PlazmaGames.Core;
using UnityEngine;

namespace WrongFloor
{
    public class EventRange : MonoBehaviour
    {
        [SerializeField] private string _eventName;

        private bool _enabled = true;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                if (_enabled) CheckPlayerInside();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && Enabled)
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

        public void CheckPlayerInside()
        {
            if (!Enabled) return;

            Collider trigger = GetComponent<Collider>();

            Collider[] hits = Physics.OverlapBox(
                trigger.bounds.center,
                trigger.bounds.extents,
                transform.rotation
            );

            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    GameManager.GetMonoSystem<IGameLogicMonoSystem>()
                        .SetInRange(_eventName, true);
                    return;
                }
            }
        }
    }
}
