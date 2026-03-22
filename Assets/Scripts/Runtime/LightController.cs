using PlazmaGames.Core;
using System;
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

        public bool ForceOnWhenDark = false;

        private float _orignalIntensity;
        private Color _orignalColor;

        public bool IsOn = true;

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
            _mr.materials[_lightMaterialIndex].SetColor("_BaseColor", color);
            _onColor = color;
            _light.color = color;
        }

        public void RevertColor()
        {
            _mr.materials[_lightMaterialIndex].SetColor("_BaseColor", _orignalColor);
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

        private void SetKeyWord<T>(Material material, string parameterBaseName, T selectedKeyword) where T : Enum
        {
            var allKeywords = Enum.GetValues(typeof(T));
            foreach (T keyword in allKeywords)
            {
                var keywordText = $"{parameterBaseName}_{keyword.ToString().ToUpper()}";

                if (keyword.Equals(selectedKeyword))
                    material.EnableKeyword(keywordText);
                else
                    material.DisableKeyword(keywordText);
            }
        }

        public void TurnOn()
        {
            IsOn = true;
            _mr.materials[_lightMaterialIndex].SetColor("_BaseColor", _onColor);
            SetKeyWord<LightingMethods>(_mr.materials[_lightMaterialIndex], "_LIGHTINGMETHOD", LightingMethods.Unlit);
            _light.gameObject.SetActive(true);
            _light.color = _onColor;
            _as.Play();
        }

        public void TurnOff(bool isOn = false)
        {
            IsOn = isOn;
            _mr.materials[_lightMaterialIndex].SetColor("_BaseColor", Color.white);
            SetKeyWord<LightingMethods>(_mr.materials[_lightMaterialIndex], "_LIGHTINGMETHOD", LightingMethods.Texel_Lit);
            _light.gameObject.SetActive(false);
            _as.Stop();
        }

        private enum LightingMethods
        {
            Unlit,
            Lit,
            Texel_Lit,
            Vertex_Lit
        }
    }
}