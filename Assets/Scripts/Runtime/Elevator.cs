using JetBrains.Annotations;
using PlazmaGames.Core;
using TMPro;
using UnityEngine;
using WrongFloor.Utilizes;

namespace WrongFloor
{
    public class Elevator : MonoBehaviour
    {
        private float _wrongYHeight = 1.07f;
        [SerializeField] private float _correctYHeight = 1.07f;
        
        [SerializeField] private Transform _doorL;
        [SerializeField] private Transform _doorR;

        [SerializeField] private TMP_Text _floorNumberText;
        
        private Promise _movePromise = null;
        private bool _movingFloor = false;
        private int _floor;
        private int _floorTarget;
        private float _floorMoveTick = 0;

        private Promise _doorPromise = null;
        private float _doorPos = 1;
        private int _doorDir = 0;

        private void Start()
        {
            _wrongYHeight = transform.position.y;
            _floor = WFGameManager.Preferences.StartFloor;
            _floorNumberText.text = _floor.ToString();
        }

        public void MoveToWrongPosition()
        {
            transform.position = transform.position.SetY(_wrongYHeight);
        }
        
        public void MoveToCorrectPosition()
        {
            transform.position = transform.position.SetY(_correctYHeight);
        }

        public Promise OpenDoors()
        {
            if (_doorPromise != null) return _doorPromise;
            _doorPromise = new Promise();
            _doorDir = -1;
            return _doorPromise;
        }
        
        public Promise CloseDoors()
        {
            if (_doorPromise != null) return _doorPromise;
            _doorPromise = new();
            _doorDir = 1;
            return _doorPromise;
        }

        public Promise MoveDown()
        {
            if (_movePromise != null) return _movePromise;
            _movePromise = new();
            _floorTarget = _floor - WFGameManager.Preferences.FloorMoveAmount;
            _floorMoveTick = 0;
            _movingFloor = true;
            return _movePromise;
        }

        private void Update()
        {
            if (_movingFloor)
            {
                _floorMoveTick += Time.deltaTime / WFGameManager.Preferences.FloorMoveSpeed;
                if (_floorMoveTick > 1.0f)
                {
                    _floorMoveTick = 0;
                    _floor -= 1;
                    _floorNumberText.text = _floor.ToString();

                    if (_floor == _floorTarget)
                    {
                        _movingFloor = false;
                        Promise.ResolveExisting(ref _movePromise);
                    }
                }
            }
            
            if (_doorDir != 0)
            {
                _doorPos += Time.deltaTime * _doorDir / WFGameManager.Preferences.DoorSpeed;

                if (_doorPos < 0 || _doorPos > 1)
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
