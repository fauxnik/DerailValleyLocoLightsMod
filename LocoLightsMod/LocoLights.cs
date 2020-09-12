using UnityEngine;
using System;

namespace LocoLightsMod
{
    public class LocoLights : MonoBehaviour
    {
        private TrainCar car;
        private LocoLightData[] exterior;
        private LocoLightData[] cab;
        private Renderer[] renderers;
        private Light[] lights;
        private Action<float>[] flickerers;
        private Transform[] cabLights;
        private Action<float>[] cabFlickerers;
        public bool IsOn { get; private set; }
        public float Direction { get; private set; }

        void Update()
        {
            if (!IsOn) { return; }

            for (int i = 0; i < exterior.Length; i++)
            {
                flickerers[i]?.Invoke(Time.deltaTime);
            }
            for (int i = 0; i < cab.Length; i++)
            { 
                cabFlickerers[i]?.Invoke(Time.deltaTime);
            }
        }

        public void Init(TrainCar car, LocoLightData[] exterior, LocoLightData[] cab)
		{
            Main.Log($"Initializing LocoLights for {car.ID}...");
            this.car = car;
            this.exterior = exterior;
            this.cab = cab;
            cabLights = new Transform[] { car.transform.Find("[cab light override]") ?? car.transform.Find("[cab light]") };
            Direction = 0f;
        }

        private void LazyInit()
		{
            Main.Log($"Secondary LocoLights initialization for {car.ID}...");
            renderers = new Renderer[exterior.Length];
            lights = new Light[exterior.Length];
            flickerers = new Action<float>[exterior.Length];
            cabFlickerers = new Action<float>[cab.Length];

            for (int i = 0; i < exterior.Length; i++)
            {
                LocoLightData datum = exterior[i];
                renderers[i] = car.transform.Find(datum.type.ToString()).gameObject.GetComponent<Renderer>();
                lights[i] = renderers[i].transform.gameObject.GetComponent<Light>();
            }

            UpdateFlickerers();
        }

        public void SetLights(bool isOn)
		{
            Main.Log($"Setting lights for {car.ID}... (prev={(IsOn ? "on" : "off")}, next={(isOn ? "on" : "off")})");
            IsOn = isOn;
            UpdateLights();
		}

        public void SetDirection(float direction)
        {
            Main.Log($"Setting direction for {car.ID}... (prev={Direction}, next={direction})");
            Direction = direction;
            UpdateFlickerers();
            UpdateLights();
		}

        private float GetDirectionalScalar(LocoLightData datum)
		{
            return Direction > 0.05f ? datum.fwd : Direction < -0.05f ? datum.rev : datum.ntl;
        }

        private void UpdateFlickerers()
		{
            if (car == null || exterior == null || cabLights == null) { return; }
            Main.Log($"Updating flickerers for {car.ID}... (direction={Direction})");

            if (renderers == null)
            {
                LazyInit();
            }

            for (int i = 0; i < exterior.Length; i++)
			{
                LocoLightData datum = exterior[i];
                float dirScalar = GetDirectionalScalar(datum);
                flickerers[i] = FlickerLight(lights[i], datum.min, datum.max, datum.rate, dirScalar);
            }
            for (int i = 0; i < cab.Length; i++)
            {
                var datum = cab[i];
                cabFlickerers[i] = FlickerLight(cabLights[i].GetComponent<Light>(), datum.min, datum.max, datum.rate);
            }
        }

        private void UpdateLights()
        {
            if (car == null || exterior == null || cabLights == null) { return; }
            Main.Log($"Updating lights for {car.ID}... (ison={IsOn})");

            if (renderers == null)
            {
                LazyInit();
            }

            for(int i = 0; i < renderers.Length; i++)
			{
                Renderer renderer = renderers[i];
                Light light = lights[i];

                light.enabled = IsOn;
                renderer.enabled = IsOn && GetDirectionalScalar(exterior[i]) > 0f;
                renderer.material.SetColor("_EmissionColor", light.color);
            }

            for (int i = 0; i < cabLights.Length; i++)
            {
                cabLights[i].gameObject.SetActive(IsOn);
            }
        }

        private Action<float> FlickerLight(Light light, float min = 0f, float max = 1f, float rate = 1f, float scalar = 1f)
        {
            float t = 100f * (float)new System.Random().NextDouble();

            Action<float> flicker = delta =>
            {
                light.intensity = scalar * ((max - min) * Mathf.PerlinNoise(t, 0f) + min);

                t += delta * rate;
            };

            // prevent flash effect when turning on or changing direction
            flicker(0);

            return flicker;
        }
    }
}
