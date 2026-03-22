using PlazmaGames.Core;
using UnityEngine;
using WrongFloor.MonoSystems;

namespace WrongFloor
{
    public class LightController : MonoBehaviour
    {
        [SerializeField] LightState _lightType;
        [SerializeField] private int _lightMaterialIndex; 

        [SerializeField] private Color _onColor;
        [SerializeField] private Color _offColor;

        [SerializeField] private MeshRenderer _mr;
        [SerializeField] private Light _light;
        [SerializeField] private AudioSource _as;

        private float _orignalIntensity;
        private Color _orignalColor;

        private void OnEnable()
        {
            GameManager.GetMonoSystem<ILightMonoSystem>().Subscribe(this, _lightType);
        }

        private void OnDisable()
        {
            GameManager.GetMonoSystem<ILightMonoSystem>().Unsubscribe(this, _lightType);
        }

        private void Awake()
        {
            _orignalIntensity = _light.intensity;
            _orignalColor = _onColor;
            TurnOn();
        }

        public void SetColor(Color color)
        {
            _onColor = color;
            _light.color = color;
        }

        public void RevertColor()
        {
            _onColor = _orignalColor;
            _light.color = _orignalColor;
        }

        public void SetIntensity(float val)
        {
            _light.intensity = val;
        }

        public void RevertIntensity()
        {
            _light.intensity = _orignalIntensity;
        }

        public void TurnOn()
        {
            _mr.materials[_lightMaterialIndex].SetColor("_BaseColor", _onColor);
            _light.gameObject.SetActive(true);
            _light.color = _onColor;
            _as.Play();
        }

        public void TurnOff()
        {
            _mr.materials[_lightMaterialIndex].SetColor("_BaseColor", _offColor);
            _light.gameObject.SetActive(false);
            _as.Stop();
        }
    }
}