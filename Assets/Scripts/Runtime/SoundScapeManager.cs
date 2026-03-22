using System.Collections;
using UnityEngine;

namespace WrongFloor
{
    public class SoundScapeManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _windAS;

        private Coroutine _windFadeRoutine;

        public void StopWind()
        {
            _windAS.Stop();
        }

        public void PlayWind()
        {
            _windAS.Play();
        }

        public void WindVolume(float targetVolume, float duration = 4f)
        {
            targetVolume = Mathf.Clamp01(targetVolume);

            if (_windFadeRoutine != null)
                StopCoroutine(_windFadeRoutine);

            _windFadeRoutine = StartCoroutine(FadeVolumeRoutine(targetVolume, duration));
        }

        private IEnumerator FadeVolumeRoutine(float targetVolume, float duration)
        {
            float startVolume = _windAS.volume;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;

                _windAS.volume = Mathf.Lerp(startVolume, targetVolume, t);
                yield return null;
            }

            _windAS.volume = targetVolume;
            _windFadeRoutine = null;
        }
    }
}
