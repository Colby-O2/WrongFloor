using PlazmaGames.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace WrongFloor
{
    public class ScreenFade : MonoBehaviour
    {
        [SerializeField] private Image _img;

        private Promise _promise;

        public Promise FadeOut(float duration)
        {
            Promise.CreateExisting(ref _promise);
            StartCoroutine(Fade(0f, 1f, duration));
            return _promise;
        }

        public Promise FadeIn(float duration)
        {
            Promise.CreateExisting(ref _promise);
            StartCoroutine(Fade(1f, 0f, duration));
            return _promise;
        }

        private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
        {
            float time = 0f;
            Color color = _img.color;

            while (time < duration)
            {
                float t = time / duration;
                if (startAlpha < endAlpha) t = 1f - t;
                t = Mathf.Pow(t, 3.0f);
                if (startAlpha < endAlpha) t = 1f - t;
                color.a = Mathf.Lerp(startAlpha, endAlpha, t);
                _img.color = color;

                time += Time.deltaTime;
                yield return null;
            }

            color.a = endAlpha;
            _img.color = color;
            Promise.ResolveExisting(ref _promise);
        }
    }
}
