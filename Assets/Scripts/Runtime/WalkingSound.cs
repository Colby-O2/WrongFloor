using UnityEngine;
using UnityEngine.Audio;

namespace WrongFloor.Behaviour
{
    [RequireComponent(typeof(VelocityTracker))]
    public class WalkingSound : MonoBehaviour
    {
        [SerializeField] private VelocityTracker _vel;

        [SerializeField] private AudioSource _as;

        public static bool Enabled = true;

        private void Awake()
        {
            if (!_vel) _vel = GetComponent<VelocityTracker>();
        }

        private void Update()
        {
            if (!Enabled) return;
            if (_as.isPlaying && _vel.SpeedInPlane < 0.01f) _as.Stop();
            else if (!_as.isPlaying && _vel.SpeedInPlane > 0.01f)
            {
                _as.time = Random.Range(0f, _as.clip.length);
                _as.Play();
            }
        }

    }
}
