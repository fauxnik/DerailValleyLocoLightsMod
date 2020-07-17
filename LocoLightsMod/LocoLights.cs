using UnityEngine;
using System.Collections.Generic;

namespace LocoLightsMod
{
    public class LocoLights : MonoBehaviour
    {
        private Renderer LDL;
        private Light LDLLight;
        private TrainCar car;
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
            }
            else
            {
                LDL.enabled = false;
                LDLLight.enabled = false;
            }

            var transform = car.transform.Find("[cab light]");
            if (transform != null)
            {
                transform.gameObject.SetActive(isOn);
            }
        }
    }
}
