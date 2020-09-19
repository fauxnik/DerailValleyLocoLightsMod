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
        public Light[] extLights { get; protected set; }
        protected Action<float>[] extFlickerers;
        public Light[] intLights { get; protected set; }
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
            IsOn = false;
            foreach (var datum in exterior)
            {
                GameObject.Destroy(GameObjectUtils.FindObject(car.gameObject, datum.name));
            }
            exterior = new LocoLightData[0];
            renderers = new Renderer[0];
            extLights = new Light[0];
            extFlickerers = new Action<float>[0];
            foreach (var datum in interior)
            {
                GameObject.Destroy(GameObjectUtils.FindObject(car.gameObject, datum.name));
            }
            interior = new LocoLightData[0];
            intLights = new Light[0];
            intFlickerers = new Action<float>[0];
        }

        public virtual void Init(TrainCar car, LocoLightData[] exterior, LocoLightData[] interior)
        {
            Main.Log($"Initializing lights for {car.ID}...");
            this.car = car;

            AddExtLights(car, exterior);

            this.interior = interior;
            intLights = new Light[interior.Length];
            intFlickerers = new Action<float>[interior.Length];

            for (int i = 0; i < interior.Length; i++)
            {
                LocoLightData datum = interior[i];
                intLights[i] = GameObjectUtils.FindObject(car.gameObject, datum.name).GetComponent<Light>();
            }

            UpdateFlickerers();
            UpdateLights();
        }

        public void AddExtLights(TrainCar car, LocoLightData[] exterior)
        {
            int basis = 0;
            if (this.exterior == null) { this.exterior = exterior; }
            else
            {
                basis = this.exterior.Length;
                this.exterior = CombineArrays<LocoLightData>(this.exterior, exterior);
            }

            if (renderers == null) { renderers = new Renderer[exterior.Length]; }
            else { renderers = ExpandArray(renderers, exterior.Length); }

            if (extLights == null) { extLights = new Light[exterior.Length]; }
            else { extLights = ExpandArray(extLights, exterior.Length); }

            if (extFlickerers == null) { extFlickerers = new Action<float>[exterior.Length]; }
            else { extFlickerers = ExpandArray(extFlickerers, exterior.Length); }

            for (int i = basis; i < this.exterior.Length; i++)
            {
                LocoLightData datum = exterior[i - basis];
                renderers[i] = GameObjectUtils.FindObject(car.gameObject, datum.name).GetComponent<Renderer>();
                extLights[i] = renderers[i].transform.gameObject.GetComponent<Light>();
            }
        }

        public void RemoveExtLight(string name)
        {
            int index = -1;
            for (int j = 0; j < exterior.Length; j++)
            {
                if (exterior[j].name == name)
                {
                    index = j;
                    break;
                }
            }
            if (index < 0) { return; }

            exterior = RemoveAt(exterior, index);
            renderers = RemoveAt(renderers, index);
            extLights = RemoveAt(extLights, index);
            extFlickerers = RemoveAt(extFlickerers, index);
        }

        private T[] CombineArrays<T>(T[] a, T[] b)
        {
            T[] c = new T[a.Length + b.Length];
            a.CopyTo(c, 0);
            b.CopyTo(c, a.Length);
            return c;
        }

        private T[] ExpandArray<T>(T[] a, int more)
        {
            T[] b = new T[a.Length + more];
            a.CopyTo(b, 0);
            return b;
        }

        private T[] RemoveAt<T>(T[] a, int index)
        {
            T[] b = new T[a.Length - 1];
            for (int i = 0; i < index; i++)
            {
                b[i] = a[i];
            }
            for (int j = index + 1; j < a.Length; j++)
            {
                b[j - 1] = a[j];
            }
            return b;
        }

        public void SetExteriorShadows(LightShadows shadows)
        {
            foreach (var light in extLights)
            {
                light.shadows = shadows;
            }
        }

        public void SetInteriorShadows(LightShadows shadows)
        {
            foreach (var light in intLights)
            {
                light.shadows = shadows;
            }
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
                if (light == null) { return; }

                light.intensity = scalar * ((max - min) * Mathf.PerlinNoise(t, 0f) + min);

                t += delta * rate;
            };

            // prevent flash effect when turning on or changing direction
            flicker(0);

            return flicker;
        }
    }
}
