using UnityEngine;
using System;

namespace LocoLightsMod
{
    public class TrainCarLights : MonoBehaviour
    {
        protected TrainCar car;
        protected LocoLightData[] exterior;
        protected LocoLightData[] interior;
        protected Renderer[] renderers;
        protected Light[] extLights;
        protected Action<float>[] extFlickerers;
        protected Light[] intLights;
        protected Action<float>[] intFlickerers;
        public virtual bool IsOn { get => true; protected set { } }

        void Update()
        {
            if (!IsOn) { return; }

            for (int i = 0; i < exterior.Length; i++)
            {
                extFlickerers[i]?.Invoke(Time.deltaTime);
            }
            for (int i = 0; i < interior.Length; i++)
            {
                intFlickerers[i]?.Invoke(Time.deltaTime);
            }
        }

        void OnDestroy()
        {
            Main.Log($"Detroying lights for {car.ID}...");
            foreach (var renderer in renderers)
            {
                GameObject.DestroyImmediate(renderer);
            }
            foreach (var light in extLights)
            {
                GameObject.DestroyImmediate(light);
            }
            foreach (var light in intLights)
            {
                GameObject.DestroyImmediate(light);
            }
        }

        public void Init(TrainCar car, LocoLightData[] exterior, LocoLightData[] interior)
        {
            Main.Log($"Initializing lights for {car.ID}...");
            this.car = car;
            this.exterior = exterior;
            this.interior = interior;
            renderers = new Renderer[exterior.Length];
            extLights = new Light[exterior.Length];
            extFlickerers = new Action<float>[exterior.Length];
            intLights = new Light[interior.Length];
            intFlickerers = new Action<float>[interior.Length];

            for (int i = 0; i < exterior.Length; i++)
            {
                LocoLightData datum = exterior[i];
                renderers[i] = car.transform.Find(datum.name).gameObject.GetComponent<Renderer>();
                extLights[i] = renderers[i].transform.gameObject.GetComponent<Light>();
            }
            for (int i = 0; i < interior.Length; i++)
            {
                LocoLightData datum = interior[i];
                intLights[i] = car.transform.Find(datum.name).gameObject.GetComponent<Light>();
            }

            UpdateFlickerers();
            UpdateLights();
        }

        protected virtual float GetDirectionalScalar(LocoLightData datum)
        {
            return 1f;
        }

        protected void UpdateFlickerers()
        {
            if (car == null || exterior == null || interior == null) { return; }
            Main.Log($"Updating flickerers for {car.ID}...");

            for (int i = 0; i < exterior.Length; i++)
            {
                var datum = exterior[i];
                float dirScalar = GetDirectionalScalar(datum);
                extFlickerers[i] = FlickerLight(extLights[i], datum.min, datum.max, datum.rate, dirScalar);
            }
            for (int i = 0; i < interior.Length; i++)
            {
                var datum = interior[i];
                intFlickerers[i] = FlickerLight(intLights[i], datum.min, datum.max, datum.rate);
            }
        }

        protected void UpdateLights()
        {
            if (car == null || exterior == null || interior == null) { return; }
            Main.Log($"Updating lights for {car.ID}...");

            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                Light light = extLights[i];

                light.enabled = IsOn;
                renderer.enabled = IsOn && GetDirectionalScalar(exterior[i]) > 0f;
                renderer.material.SetColor("_EmissionColor", light.color);
            }

            for (int i = 0; i < intLights.Length; i++)
            {
                intLights[i].enabled = IsOn;
            }
        }

        protected Action<float> FlickerLight(Light light, float min = 0f, float max = 1f, float rate = 1f, float scalar = 1f)
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
