using JetBrains.Annotations;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using TMPro;
using UnityEngine;
using WrongFloor.Utilizes;
using AudioType = PlazmaGames.Audio.AudioType;

namespace WrongFloor
{
    public class Elevator : MonoBehaviour
    {
        private float _wrongYHeight = 1.07f;
        [SerializeField] private float _correctYHeight = 1.07f;
        [SerializeField] private AudioClip _crashSound;
        [SerializeField] private AudioClip _openSound;
        [SerializeField] private AudioClip _closeSound;
        [SerializeField] private AudioSource _mainSource;
        [SerializeField] private AudioSource _stopSource;
        
        [SerializeField] private GameObject _doorBlock;
        [SerializeField] private Transform _doorL;
        [SerializeField] private Transform _doorR;

        [SerializeField] private TMP_Text _floorNumberText;
        
        private Promise _movePromise = null;
        private bool _movingFloor = false;
        private int _floor;
        private int _floorTarget;
        private float _floorMoveTick = 0;
        private float _moveStopTime;

        private bool _falling = false;
        private float _fallT = 0;
        private bool _didOpenDoorsOnFall = false;

        private Promise _doorPromise = null;
        private float _doorPos = 1;
        private int _doorDir = 0;

        private void Start()
        {
            _wrongYHeight = transform.position.y;
            _floor = WFGameManager.Preferences.StartFloor;
            _floorNumberText.text = _floor.ToString();
        }

        public void MoveToWrongPosition(bool bringPlayer = false)
        {
            float diff = _wrongYHeight - transform.position.y;
            transform.position = transform.position.SetY(_wrongYHeight);
            if (bringPlayer)
            {
                WFGameManager.Player.Move(new Vector3(0, diff, 0));
            }
        }
        
        public void MoveToCorrectPosition()
        {
            transform.position = transform.position.SetY(_correctYHeight);
        }

        public Promise OpenDoors()
        {
            if (_doorPromise != null) return _doorPromise;
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(_openSound, AudioType.Sfx, false);
            _doorDir = -1;
            _doorPromise = new Promise();
            return _doorPromise;
        }
        
        public Promise CloseDoors()
        {
            if (_doorPromise != null) return _doorPromise;
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(_closeSound, AudioType.Sfx, false);
            _doorDir = 1;
            _doorPromise = new();
            return _doorPromise;
        }

        public Promise MoveDown()
        {
            if (_movePromise != null) return _movePromise;
            float vol = GameManager.GetMonoSystem<IAudioMonoSystem>().GetOverallVolume();
            _mainSource.volume = vol;
            _mainSource.Play();
            _moveStopTime = 0;
            _floorTarget = _floor - WFGameManager.Preferences.FloorMoveAmount;
            _floorMoveTick = 0;
            _movingFloor = true;
            _movePromise = new();
            return _movePromise;
        }

        public Promise FallElevator()
        {
            if (_movePromise != null) return _movePromise;
            float vol = GameManager.GetMonoSystem<IAudioMonoSystem>().GetOverallVolume();
            _mainSource.volume = vol;
            _mainSource.clip = _crashSound;
            _mainSource.Play();
            _movePromise = new();
            _falling = true;
            _didOpenDoorsOnFall = false;
            _fallT = 0;
            _doorBlock.SetActive(true);
            WFGameManager.Player.transform.parent = transform;
            WFGameManager.Player.GravityScale = 0.0f;
            return _movePromise;
        }

        private void Update()
        {
            if (_falling)
            {
                _fallT += Time.deltaTime;
                if (!_didOpenDoorsOnFall && _fallT > WFGameManager.Preferences.ElevatorFallElevatorOpenTime)
                {
                    _didOpenDoorsOnFall = true;
                    OpenDoors();
                }

                float speed = WFGameManager.Preferences.ElevatorFallInitialSpeed;
                float t = (_fallT - WFGameManager.Preferences.ElevatorFallSpeedUpTime) / WFGameManager.Preferences.ElevatorFallTime;
                if (t >= 0)
                {
                    speed = Mathf.Lerp(speed, WFGameManager.Preferences.ElevatorFallMaxSpeed, Mathf.Clamp01(t));
                }

                if (t > 1.0)
                {
                    Debug.Log("MAX SPEED");
                }
                
                transform.Translate(Vector3.down * (speed * Time.deltaTime));
                WFGameManager.Player.MoveToY(transform.position.y);

                if (_mainSource.isPlaying == false)
                {
                    _falling = false;
                    Promise.ResolveExisting(ref _movePromise);
                }
            }
            
            if (_movingFloor)
            {
                _floorMoveTick += Time.deltaTime / WFGameManager.Preferences.ElevatorMoveSpeed;
                if (_floor == _floorTarget)
                {
                    _moveStopTime += Time.deltaTime;
                    const float crossFadeTime = 0.2f;
                    float t = _moveStopTime / crossFadeTime;
                    float vol = GameManager.GetMonoSystem<IAudioMonoSystem>().GetOverallVolume();
                    _mainSource.volume = vol * (1.0f - t);
                    _stopSource.volume = vol * t;
                    if (_stopSource.isPlaying == false)
                    {
                        _mainSource.Stop();
                        _movingFloor = false;
                    }
                }
                else if (_floorMoveTick > 1.0f)
                {
                    _floorMoveTick = 0;
                    _floor -= 1;
                    _floorNumberText.text = _floor.ToString();

                    if (_floor == _floorTarget)
                    {
                        _stopSource.volume = 0;
                        _stopSource.Play();
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
