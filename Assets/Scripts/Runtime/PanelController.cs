using UnityEngine;

namespace WrongFloor
{
    public class PanelController : MonoBehaviour
    {
        [SerializeField] private AudioSource _as;
        [SerializeField] private AudioClip _warning;
        [SerializeField] private AudioClip _crash;

        public void PlayWarning()
        {
            if (_as && _warning) _as.PlayOneShot(_warning);
        }

        public void PlayCrash()
        {
            if (_as && _crash) _as.PlayOneShot(_crash);
        }
    }
}
