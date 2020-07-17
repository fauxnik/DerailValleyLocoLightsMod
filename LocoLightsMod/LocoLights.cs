using UnityEngine;
using System.Collections.Generic;
using System;

namespace LocoLightsMod
{
    public class LocoLights : MonoBehaviour
    {
        private Renderer FHL_R;
        private Light FHL_Light;
        private TrainCar car;
        private Action<float> HeadLightFlicker;
        private Action<float> CabLightFlicker;
        public bool isOn = false;

        public void ToggleHeadLight(TrainCar c)
        {
            car = c;

            if (FHL_R == null)
            {
                FHL_R = car.transform.Find("FHL").gameObject.GetComponent<Renderer>();
                FHL_Light = FHL_R.transform.gameObject.GetComponent<Light>();
            }

            isOn = !isOn;
            if (isOn)
            {
                FHL_R.enabled = true;
                FHL_Light.enabled = true;
                Main.Update += HeadLightFlicker = FlickerLight(FHL_Light, 1f, 2.25f, 4f);
            }
            else
            {
                FHL_R.enabled = false;
                FHL_Light.enabled = false;
                Main.Update -= HeadLightFlicker;
                HeadLightFlicker = null;
            }

            var transform = car.transform.Find("[cab light]");
            if (transform != null)
            {
                transform.gameObject.SetActive(isOn);
                if (isOn)
				{
                    Light cabLight = transform.GetComponent<Light>();
                    Main.Update += CabLightFlicker = FlickerLight(cabLight, 0.015f, 0.2f, 4f);
				}
				else
				{
                    Main.Update -= CabLightFlicker;
                    CabLightFlicker = null;
				}
            }
        }

        private Action<float> FlickerLight(Light light, float min = 0f, float max = 1f, float scale = 1f, int factor = 1)
        {
            float t = 0;
            factor = Math.Max(factor, 1);

            return delta =>
            {
                float multiplier = 1f;

                for (int i = 0; i < factor; i++) { multiplier *= Mathf.PerlinNoise(t, 0f); }

                light.intensity = (max - min) * multiplier + min;

                t += delta * scale;
            };
        }
    }
}
