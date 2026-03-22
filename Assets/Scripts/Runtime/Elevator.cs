using JetBrains.Annotations;
using PlazmaGames.Core;
using TMPro;
using UnityEngine;
using WrongFloor.Utilizes;

namespace WrongFloor
{
    public class Elevator : MonoBehaviour
    {
        [SerializeField] private Transform _doorL;
        [SerializeField] private Transform _doorR;

        [SerializeField] private TMP_Text _doorNumber;

        private Promise _doorPromise = null;
        private float _doorPos = 1;
        private int _doorDir = 0;

        public Promise OpenDoors()
        {
            if (_doorPromise != null) return _doorPromise;
            _doorPromise = new Promise();
            _doorDir = 1;
            return _doorPromise;
        }
        
        public Promise CloseDoors()
        {
            if (_doorPromise != null) return _doorPromise;
            _doorPromise = new Promise();
            _doorDir = -1;
            return _doorPromise;
        }

        private void Update()
        {
            if (_doorDir != 0)
            {
                _doorPos += Time.deltaTime * _doorDir / WFGameManager.Preferences.DoorSpeed;

                if (_doorPos < 0 || _doorDir > 1)
                {
                    _doorPos = Mathf.Clamp01(_doorPos);
                    _doorDir = 0;
                    Promise.ResolveExisting(ref _doorPromise);
                }

                float t = 1.0f - _doorPos;
                t = 1.0f - t * t;
                _doorR.localScale = _doorR.localScale.SetZ(1.0f + t);
                _doorL.localScale = _doorL.localScale.SetZ(1.0f + t);
            }
        }
    }
}
