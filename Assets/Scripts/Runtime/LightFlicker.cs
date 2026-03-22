using PlazmaGames.Attribute;
using UnityEngine;
using System.Collections;

namespace WrongFloor
{
    public class LightFlicker : MonoBehaviour
    {
        [SerializeField] private LightController _lc;
        [SerializeField] private Vector2 _flickerInterval;
        [SerializeField, Range(0f, 1f)] private float _flickerChance;
        [SerializeField] private Vector2 _flickerDuration;
        [SerializeField, Range(0f, 1f)] private float _glitchChance;
        [SerializeField] private Vector2 _glitchDuration;

        [SerializeField, ReadOnly] private bool _isOn = true;

        private IEnumerator Flicker()
        {
            while (this)
            {
                if (!_lc.IsOn)
                {
                    yield return null;
                    continue;
                }

                float wait = Random.Range(_flickerInterval.x, _flickerInterval.y);
                yield return new WaitForSeconds(wait);

                if (!_lc.IsOn)
                {
                    yield return null;
                    continue;
                }

                if (Random.value <= _glitchChance)
                {
                    _isOn = false;
                    _lc.TurnOff(true);
                    float offTime = Random.Range(_glitchDuration.x, _glitchDuration.y);
                    yield return new WaitForSeconds(offTime);
                    _isOn = true;
                    _lc.TurnOn();
                }
                if (Random.value <= _flickerChance)
                {
                    _isOn = false;
                    _lc.TurnOff(true);
                    float offTime = Random.Range(_flickerDuration.x, _flickerDuration.y);
                    yield return new WaitForSeconds(offTime);
                    _isOn = true;
                    _lc.TurnOn();
                }
            }
        }

        private void Awake()
        {
            if (!_lc) _lc = GetComponent<LightController>();
            StartCoroutine(Flicker());
        }
    }
}
