using UnityEngine;
using System.Collections.Generic;

namespace LocoLightsMod
{
    public class LocoLights : MonoBehaviour
    {
        private Renderer LDL;
        private Light LDLLight;
        private TrainCar car;
        private IEnumerator<object> HeadLightFlicker;
        private IEnumerator<object> CabLightFlicker;
        public bool isOn = false;

        public void ToggleHeadLight(TrainCar c)
        {
            car = c;

            if (LDL == null)
            {
                LDL = car.transform.Find("LDL").gameObject.GetComponent<Renderer>();
                LDLLight = LDL.transform.gameObject.GetComponent<Light>();
            }

            isOn = !isOn;
            if (isOn)
            {
                LDL.enabled = true;
                LDLLight.enabled = true;
                HeadLightFlicker = FlickerLight(LDLLight);
                car.StartCoroutine(HeadLightFlicker);
            }
            else
            {
                LDL.enabled = false;
                LDLLight.enabled = false;
                car.StopCoroutine(HeadLightFlicker);
                HeadLightFlicker = null;
            }

            var transform = car.transform.Find("[cab light]");
            if (transform != null)
            {
                transform.gameObject.SetActive(isOn);
                if (isOn)
				{
                    CabLightFlicker = FlickerLight(transform.GetComponent<Light>());
                    car.StartCoroutine(CabLightFlicker);
				}
				else
				{
                    car.StopCoroutine(CabLightFlicker);
                    CabLightFlicker = null;
				}
            }
        }

        private IEnumerator<object> FlickerLight(Light light, float min = 0.7f, float max = 1.0f)
        {
            int x = 0;
            int y = 0;
            bool isRevX = false;
            bool isRevY = true;

            min = Mathf.Clamp(min, 0f, 1f);
            max = Mathf.Clamp(max, 0f, 1f);

            while (true)
            {
                yield return WaitFor.SecondsRealtime(0.025f);

                light.intensity = (max - min) * Mathf.PerlinNoise(x, y) + min;

                x += isRevX ? -1 : 1;

                if (x + 1 > int.MaxValue || x - 1 < 0)
                {
                    isRevX = !isRevX;
                    y += isRevY ? -1 : 1;

                    if (y + 1 > int.MaxValue || y - 1 < 0)
                    {
                        isRevY = !isRevY;
                    }
                }
            }
        }
    }
}
