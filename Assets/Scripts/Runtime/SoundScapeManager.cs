using System.Collections;
using UnityEngine;

namespace WrongFloor
{
    public class SoundScapeManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _windAS;
        [SerializeField] private AudioSource _heartAS;
        [SerializeField] private AudioSource _ambienceAS;
        [SerializeField] private AudioSource _jumpscareAS;

        private Coroutine _windFadeRoutine;
        private Coroutine _heartFadeRoutine;
        private Coroutine _ambienceFadeRoutine;

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

            _windFadeRoutine = StartCoroutine(FadeVolumeRoutine(targetVolume, duration, _windAS, _windFadeRoutine));
        }

        public void StopHeart()
        {
            _heartAS.Stop();
        }

        public void PlayHeart()
        {
            _heartAS.Play();
        }

        public void PlayJumpScare()
        {
            _jumpscareAS.PlayOneShot(_jumpscareAS.clip);
        }

        public void HeartVolume(float targetVolume, float duration = 4f)
        {
            targetVolume = Mathf.Clamp01(targetVolume);

            if (_heartFadeRoutine != null)
                StopCoroutine(_heartFadeRoutine);

            _heartFadeRoutine = StartCoroutine(FadeVolumeRoutine(targetVolume, duration, _heartAS, _heartFadeRoutine));
        }

        public void StopAmbience()
        {
            _ambienceAS.Stop();
        }

        public void PlayAmbience()
        {
            _ambienceAS.Play();
        }

        public void AmbienceVolume(float targetVolume, float duration = 4f)
        {
            targetVolume = Mathf.Clamp01(targetVolume);

            if (_ambienceFadeRoutine != null)
                StopCoroutine(_ambienceFadeRoutine);

            _ambienceFadeRoutine = StartCoroutine(FadeVolumeRoutine(targetVolume, duration, _ambienceAS, _ambienceFadeRoutine));
        }

        private IEnumerator FadeVolumeRoutine(float targetVolume, float duration, AudioSource audioSource, Coroutine coroutine)
        {
            float startVolume = audioSource.volume;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;

                audioSource.volume = Mathf.Lerp(startVolume, targetVolume, t);
                yield return null;
            }

            audioSource.volume = targetVolume;
            coroutine = null;
        }
    }
}
