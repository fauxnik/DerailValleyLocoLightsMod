using UnityEngine;
using System;

namespace LocoLightsMod
{
    public class LocoLights : MonoBehaviour
    {
        private TrainCar car;
        private LocoLightData[] exterior;
        private LocoLightData cab;
        private Renderer[] renderers;
        private Light[] lights;
        private Action<float>[] flickerers;
        private Transform cabLight;
        private Action<float> cabFlickerer;
        public bool IsOn { get; private set; }
        public float Direction { get; private set; }

        public void Init(TrainCar car, LocoLightData[] exterior, LocoLightData cab)
		{
            Debug.Log("init train car " + car.logicCar.ID);
            this.car = car;
            this.exterior = exterior;
            this.cab = cab;
            cabLight = car.transform.Find("[cab light override]") ?? car.transform.Find("[cab light]");
            Direction = 0.5f;
        }

        public void SetLights(float ctrlValue)
		{
            IsOn = ctrlValue > 0.5f;
            UpdateLights();
		}

        public void SetDirection(float direction)
		{
            Direction = direction;
            UpdateFlickerers();
            UpdateLights();
		}

        private float GetDirectionalScalar(LocoLightData datum)
		{
            return Direction > 0.55 ? datum.fwd : Direction < 0.45 ? datum.rev : datum.ntl;
        }

        private void UpdateFlickerers()
		{
            Debug.Log("updating flickerers (direction=" + Direction + ")");
            for(int i = 0; i < exterior.Length; i++)
			{
                LocoLightData datum = exterior[i];
                if (flickerers[i] != null) { Main.Update -= flickerers[i]; }
                float dirScalar = GetDirectionalScalar(datum);
                flickerers[i] = FlickerLight(lights[i], datum.min, datum.max, datum.rate, dirScalar);
            }
            if (cabFlickerer != null) { Main.Update -= cabFlickerer; }
            cabFlickerer = FlickerLight(cabLight.GetComponent<Light>(), cab.min, cab.max, cab.rate);
        }

        private void UpdateLights()
        {
            if (car == null || exterior == null || cabLight == null) { return; }
            Debug.Log("updating lights (ison=" + IsOn + ")");

            if (renderers == null)
            {
                renderers = new Renderer[exterior.Length];
                lights = new Light[exterior.Length];
                flickerers = new Action<float>[exterior.Length];

                for (int i = 0; i < exterior.Length; i++)
				{
                    LocoLightData datum = exterior[i];
                    renderers[i] = car.transform.Find(datum.type.ToString()).gameObject.GetComponent<Renderer>();
                    lights[i] = renderers[i].transform.gameObject.GetComponent<Light>();
				}

                UpdateFlickerers();
            }

            for(int i = 0; i < renderers.Length; i++)
			{
                Renderer renderer = renderers[i];
                Light light = lights[i];
                Action<float> flickerer = flickerers[i];
                Color c = light.color;

                light.enabled = IsOn;
                renderer.enabled = IsOn && GetDirectionalScalar(exterior[i]) > 0f;
                renderer.material.SetColor("_EmissionColor", new Vector4(c.r, c.g, c.b, 0) * 200f);

                if (IsOn) { Main.Update += flickerer; }
                else { Main.Update -= flickerer; }
            }

            cabLight.gameObject.SetActive(IsOn);

            if (IsOn) { Main.Update += cabFlickerer; }
            else { Main.Update -= cabFlickerer; }
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

        public static void SetupLights(TrainCar c)
        {
            TrainCar car = c;
            if (car == null) { return; }

            // Front Head Light
            GameObject go;
            Renderer r;
            Light l;

            float offsetX;
            float offsetY;
            float offsetZ;
            float offsetθ;
            Vector3 euler = car.transform.rotation.eulerAngles;

            switch (car.carType)
            {
                case TrainCarType.LocoSteamHeavy:
                    car.transform.gameObject.AddComponent<LocoLights>();
                    car.transform.gameObject.GetComponent<LocoLights>().Init(car, new LocoLightData[]
                    {
                        new LocoLightData(LocoLightType.FHL, 4f, 0.75f, 1.75f)
                    }, new LocoLightData(LocoLightType.cab, 5f, 0.015f, 0.25f));

                    // Front Head Light
                    go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.name = LocoLightType.FHL.ToString();
                    GameObject.Destroy(go.GetComponent<BoxCollider>());
                    go.transform.position = car.transform.position;
                    go.transform.rotation = Quaternion.Euler(euler.x + 10f, euler.y, euler.z);
                    go.transform.localScale = new Vector3(0.28f, 0.28f, 0.05f);
                    r = go.GetComponent<Renderer>();
                    r.material.color = new Color32(255, 255, 255, 0);
                    StandardShaderUtils.ChangeRenderMode(r.material, StandardShaderUtils.BlendMode.Emission);
                    r.enabled = false;
                    l = go.AddComponent<Light>();
                    l.shadows = LightShadows.Soft;
                    l.type = LightType.Spot;
                    l.innerSpotAngle = 14;
                    l.spotAngle = 42;
                    l.color = new Color32(255, 198, 111, 255);
                    l.range = 98;
                    l.enabled = false;

                    go.transform.position += car.transform.forward * 10.86f;
                    go.transform.position += car.transform.up * 3.58f;

                    go.transform.SetParent(car.transform, true);

                    // default cab light: Color32(255, 253, 240, 255)
                    car.transform.Find("[cab light]").GetComponent<Light>().color = new Color32(255, 179, 63, 255);
                    break;

                case TrainCarType.LocoShunter:
                    car.transform.gameObject.AddComponent<LocoLights>();
                    car.transform.gameObject.GetComponent<LocoLights>().Init(car, new LocoLightData[]
                    {
                        new LocoLightData(LocoLightType.LFDL, 4f, 1f, 1.75f, 1f, 0.15f, 0.15f),
                        new LocoLightData(LocoLightType.RFDL, 4f, 1f, 1.75f, 1f, 0.15f, 0.15f),
                        new LocoLightData(LocoLightType.LRDL, 4f, 1f, 1.75f, 0.15f, 1f, 0.15f),
                        new LocoLightData(LocoLightType.RRDL, 4f, 1f, 1.75f, 0.15f, 1f, 0.15f)
                    }, new LocoLightData(LocoLightType.cab, 2f, 0.1f, 0.15f));

                    // Left Front Ditch Light
                    go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.name = LocoLightType.LFDL.ToString();
                    GameObject.Destroy(go.GetComponent<BoxCollider>());
                    go.transform.position = car.transform.position;
                    go.transform.rotation = Quaternion.Euler(euler.x + 5f, euler.y, euler.z);
                    go.transform.localScale = new Vector3(0.21f, 0.21f, 0.05f);
                    r = go.GetComponent<Renderer>();
                    r.material.color = new Color32(255, 255, 255, 0);
                    StandardShaderUtils.ChangeRenderMode(r.material, StandardShaderUtils.BlendMode.Emission);
                    r.enabled = false;
                    l = go.AddComponent<Light>();
                    l.shadows = LightShadows.Soft;
                    l.type = LightType.Spot;
                    l.innerSpotAngle = 17;
                    l.spotAngle = 51;
                    l.color = new Color32(255, 251, 225, 255);
                    l.range = 52;
                    l.enabled = false;

                    offsetX = 0.65f;
                    go.transform.position += car.transform.forward * 3.175f;
                    go.transform.position += car.transform.up * 1.528f;
                    go.transform.position += -car.transform.right * offsetX;

                    go.transform.SetParent(car.transform, true);

                    // Right Front Ditch Light
                    go = GameObject.Instantiate(go, go.transform.position, go.transform.rotation);
                    go.name = LocoLightType.RFDL.ToString();
                    (r = go.GetComponent<Renderer>()).enabled = false;
                    (l = go.GetComponent<Light>()).enabled = false;

                    go.transform.position += car.transform.right * 2f * offsetX;

                    go.transform.SetParent(car.transform, true);

                    // Left Rear Ditch Light
                    go = GameObject.Instantiate(go, car.transform.position, Quaternion.Euler(euler.x + 5f, euler.y + 180f, euler.z));
                    go.name = LocoLightType.LRDL.ToString();
                    (r = go.GetComponent<Renderer>()).enabled = false;
                    (l = go.GetComponent<Light>()).enabled = false;

                    offsetX = 0.65f;
                    go.transform.position += -car.transform.forward * 3.167f;
                    go.transform.position += car.transform.up * 1.928f;
                    go.transform.position += -car.transform.right * offsetX;

                    go.transform.SetParent(car.transform, true);

                    // Right Rear Ditch Light
                    go = GameObject.Instantiate(go, go.transform.position, go.transform.rotation);
                    go.name = LocoLightType.RRDL.ToString();
                    (r = go.GetComponent<Renderer>()).enabled = false;
                    (l = go.GetComponent<Light>()).enabled = false;

                    go.transform.position += car.transform.right * 2f * offsetX;

                    go.transform.SetParent(car.transform, true);

                    // default cab light: Color32(255, 253, 240, 255)
                    car.transform.Find("[cab light]").GetComponent<Light>().color = new Color32(255, 251, 225, 255);
                    break;

                case TrainCarType.LocoDiesel:
                    car.transform.gameObject.AddComponent<LocoLights>();
                    car.transform.gameObject.GetComponent<LocoLights>().Init(car, new LocoLightData[]
                    {
                        new LocoLightData(LocoLightType.FHL, 4f, 1.25f, 2f, 1f, 0.15f, 0.15f),
                        new LocoLightData(LocoLightType.RHL, 4f, 1.25f, 2f, 0.15f, 1f, 0.15f),
                        new LocoLightData(LocoLightType.LFDL, 4f, 1f, 1.75f, 1f, 0.15f, 0.15f),
                        new LocoLightData(LocoLightType.RFDL, 4f, 1f, 1.75f, 1f, 0.15f, 0.15f),
                        new LocoLightData(LocoLightType.LRDL, 4f, 1f, 1.75f, 0.15f, 1f, 0.15f),
                        new LocoLightData(LocoLightType.RRDL, 4f, 1f, 1.75f, 0.15f, 1f, 0.15f)
                        //new LocoLightData(LocoLightType.FWL, 3f, 0.5f, 0.6f, 0f, 1f, 0f),
                        //new LocoLightData(LocoLightType.RWL, 3f, 0.5f, 0.6f, 1f, 0f, 0f)
                    }, new LocoLightData(LocoLightType.cab, 2f, 0.375f, 0.5f));

                    // Front Head Light
                    go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.name = LocoLightType.FHL.ToString();
                    GameObject.Destroy(go.GetComponent<BoxCollider>());
                    go.transform.position = car.transform.position;
                    go.transform.rotation = Quaternion.Euler(euler.x + 10f, euler.y, euler.z);
                    go.transform.localScale = new Vector3(0.19f, 0.19f, 0.05f);
                    r = go.GetComponent<Renderer>();
                    StandardShaderUtils.ChangeRenderMode(r.material, StandardShaderUtils.BlendMode.Emission);
                    r.enabled = false;
                    l = go.AddComponent<Light>();
                    l.shadows = LightShadows.Soft;
                    l.type = LightType.Spot;
                    l.innerSpotAngle = 14;
                    l.spotAngle = 42;
                    l.color = new Color32(255, 251, 225, 255);
                    l.range = 98;
                    l.enabled = false;

                    offsetY = 3.765f;
                    offsetZ = 7.128f;
                    go.transform.position += car.transform.forward * offsetZ;
                    go.transform.position += car.transform.up * offsetY;

                    go.transform.SetParent(car.transform, true);

                    // Left Front Ditch Light
                    go = GameObject.Instantiate(go, go.transform.position, go.transform.rotation);
                    go.name = LocoLightType.LFDL.ToString();
                    (r = go.GetComponent<Renderer>()).enabled = false;
                    (l = go.GetComponent<Light>()).enabled = false;
                    l.innerSpotAngle = 17;
                    l.spotAngle = 51;
                    l.range = 26;

                    offsetX = 0.6115f;
                    offsetθ = 0; // 13.5f;
                    go.transform.position += car.transform.forward * (8.328f - offsetZ);
                    go.transform.position += car.transform.up * (2.007f - offsetY);
                    go.transform.position += -car.transform.right * offsetX;
                    go.transform.rotation = Quaternion.Euler(euler.x + 7.5f, euler.y - offsetθ, euler.z);
                    go.transform.localScale = new Vector3(0.165f, 0.165f, 0.05f);

                    go.transform.SetParent(car.transform, true);

                    // Right Front Ditch Light
                    go = GameObject.Instantiate(go, go.transform.position, go.transform.rotation);
                    go.name = LocoLightType.RFDL.ToString();
                    (r = go.GetComponent<Renderer>()).enabled = false;
                    (l = go.GetComponent<Light>()).enabled = false;

                    go.transform.position += car.transform.right * 2f * offsetX;
                    go.transform.rotation = Quaternion.Euler(euler.x + 7.5f, euler.y + offsetθ, euler.z);

                    go.transform.SetParent(car.transform, true);

                    // Rear Head Light
                    go = GameObject.Instantiate(go, car.transform.position, Quaternion.Euler(euler.x + 10f, euler.y + 180f, euler.z));
                    go.name = LocoLightType.RHL.ToString();
                    (r = go.GetComponent<Renderer>()).enabled = false;
                    (l = go.GetComponent<Light>()).enabled = false;
                    l.innerSpotAngle = 14;
                    l.spotAngle = 42;
                    l.range = 98;

                    offsetY = 3.43f;
                    offsetZ = 8.23f;
                    go.transform.position += -car.transform.forward * offsetZ;
                    go.transform.position += car.transform.up * offsetY;
                    go.transform.localScale = new Vector3(0.21f, 0.21f, 0.05f);

                    go.transform.SetParent(car.transform, true);

                    // Left Rear Ditch Light
                    go = GameObject.Instantiate(go, go.transform.position, go.transform.rotation);
                    go.name = LocoLightType.LRDL.ToString();
                    (r = go.GetComponent<Renderer>()).enabled = false;
                    (l = go.GetComponent<Light>()).enabled = false;
                    l.innerSpotAngle = 17;
                    l.spotAngle = 51;
                    l.range = 26;

                    offsetX = 0.615f;
                    offsetθ = 0;
                    go.transform.position += -car.transform.forward * (8.197f - offsetZ);
                    go.transform.position += car.transform.up * (2.007f - offsetY);
                    go.transform.position += -car.transform.right * offsetX;
                    go.transform.rotation = Quaternion.Euler(euler.x + 7.5f, euler.y + 180f + offsetθ, euler.z);
                    go.transform.localScale = new Vector3(0.165f, 0.165f, 0.05f);

                    go.transform.SetParent(car.transform, true);

                    // Right Rear Ditch Light
                    go = GameObject.Instantiate(go, go.transform.position, go.transform.rotation);
                    go.name = LocoLightType.RRDL.ToString();
                    (r = go.GetComponent<Renderer>()).enabled = false;
                    (l = go.GetComponent<Light>()).enabled = false;

                    go.transform.position += car.transform.right * 2f * offsetX;
                    go.transform.rotation = Quaternion.Euler(euler.x + 7.5f, euler.y + 180f - offsetθ, euler.z);

                    go.transform.SetParent(car.transform, true);

                    /*
                    // Rear Warning Light
                    go = GameObject.Instantiate(go, car.transform.position, Quaternion.Euler(euler.x, euler.y + 180f, euler.z));
                    go.name = LocoLightType.RWL.ToString();
                    (r = go.GetComponent<Renderer>()).enabled = false;
                    (l = go.GetComponent<Light>()).enabled = false;
                    l.color = Color.red;
                    l.innerSpotAngle = 42;
                    l.spotAngle = 84;
                    l.range = 26;

                    go.transform.position += -car.transform.forward * (offsetZ + 0.053f);
                    go.transform.position += car.transform.up * 3.117f;
                    go.transform.localScale = new Vector3(0.21f, 0.21f, 0.05f);

                    go.transform.SetParent(car.transform, true);

                    // Front Warning Light
                    go = GameObject.Instantiate(go, car.transform.position, car.transform.rotation);
                    go.name = LocoLightType.FWL.ToString();
                    (r = go.GetComponent<Renderer>()).enabled = false;
                    (l = go.GetComponent<Light>()).enabled = false;

                    go.transform.position += car.transform.forward * 8.488f;
                    go.transform.position += car.transform.up * 2.83f;
                    go.transform.localScale = new Vector3(0.21f, 0.21f, 0.05f);

                    go.transform.SetParent(car.transform, true);
                    */

                    // this is dumb, but DE6 cab light is fucked
                    Transform cabLight = car.transform.Find("[cab light]");
                    go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.name = "[cab light override]";
                    r = go.GetComponent<Renderer>();
                    r.enabled = false;
                    l = go.AddComponent<Light>();
                    l.type = LightType.Point;
                    // default cab light: Color32(255, 253, 240, 255)
                    l.color = new Color32(255, 251, 225, 255);
                    l.range = 6.6f;
                    l.enabled = false;
                    go.transform.position = cabLight.position;
                    go.transform.SetParent(cabLight, true);
                    break;
            }
        }
    }
}
