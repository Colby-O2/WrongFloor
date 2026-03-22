using System.Collections.Generic;
using UnityEngine;

namespace WrongFloor.MonoSystems
{
    public enum LightState
    {
        Nomral,
        Emergency,
        Off
    }

    public class LightMonoSystem : MonoBehaviour, ILightMonoSystem
    {
        private List<LightController> _normalLights = new List<LightController>();
        private List<LightController> _emergencyLights = new List<LightController>();

        private void NormalLightToggle(bool isOn, bool isDark = false)
        {
            foreach (LightController light in _normalLights)
            {
                if (isOn || (isDark && light.ForceOnWhenDark)) light.TurnOn();
                else light.TurnOff();
            }
        }

        private void EmergencyLightToggle(bool isOn, bool isDark = false)
        {
            foreach (LightController light in _emergencyLights)
            {
                if (isOn || (isDark && light.ForceOnWhenDark)) light.TurnOn();
                else light.TurnOff();
            }
        }

        private void NormalLightSetColor(Color? color)
        {
            foreach (LightController light in _normalLights)
            {
                if (color != null) light.SetColor(color ?? Color.white);
                else light.RevertColor();
            }
        }

        private void EmergencyLightSetColor(Color? color)
        {
            foreach (LightController light in _emergencyLights)
            {
                if (color != null) light.SetColor(color ?? Color.white);
                else light.RevertColor();
            }
        }

        private void NormalLightSetIntensity(float? val)
        {
            foreach (LightController light in _normalLights)
            {
                if (val != null) light.SetIntensity(val ?? 1f);
                else light.RevertIntensity();
            }
        }

        private void EmergencyLightSetIntensity(float? val)
        {
            foreach (LightController light in _emergencyLights)
            {
                if (val != null) light.SetIntensity(val ?? 1f);
                else light.RevertIntensity(); 
            }
        }

        public void Subscribe(LightController lc, LightState state)
        {
            if (state == LightState.Nomral) _normalLights.Add(lc);
            else if (state == LightState.Emergency) _emergencyLights.Add(lc);
        }

        public void Unsubscribe(LightController lc, LightState state)
        {
            if (state == LightState.Nomral) _normalLights.Remove(lc);
            else if (state == LightState.Emergency) _emergencyLights.Remove(lc);
        }

        public void Toggle(LightState state)
        {
            switch (state)
            {
                case LightState.Nomral:
                    NormalLightToggle(true);
                    EmergencyLightToggle(false);
                    break;
                case LightState.Emergency:
                    NormalLightToggle(false);
                    EmergencyLightToggle(true);
                    break;
                case LightState.Off:
                    NormalLightToggle(false, isDark: true);
                    EmergencyLightToggle(false, isDark: true);
                    break;
            }
        }

        public void SetColor(Color? color, LightState state)
        {
            switch (state)
            {
                case LightState.Nomral:
                    NormalLightSetColor(color);
                    break;
                case LightState.Emergency:
                    EmergencyLightSetColor(color);
                    break;
 
            }
        }

        public void SetIntensity(float? val, LightState state)
        {
            switch (state)
            {
                case LightState.Nomral:
                    NormalLightSetIntensity(val);
                    break;
                case LightState.Emergency:
                    EmergencyLightSetIntensity(val);
                    break;

            }
        }
    }
}