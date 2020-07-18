using UnityEngine;
using System.Collections.Generic;
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
            cabLight = car.transform.Find("[cab light]");
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

        private void UpdateFlickerers()
		{
            Debug.Log("updating flickerers (direction=" + Direction + ")");
            for(int i = 0; i < exterior.Length; i++)
			{
                LocoLightData datum = exterior[i];
                if (flickerers[i] != null) { Main.Update -= flickerers[i]; }
                float dirScale = Direction > 0.55 ? datum.scaleFwd : Direction < 0.45 ? datum.scaleRev : datum.scaleNtl;
                flickerers[i] = FlickerLight(lights[i], datum.min, datum.max, datum.rate, dirScale);
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

                renderer.enabled = IsOn;
                light.enabled = IsOn;

                if (IsOn) { Main.Update += flickerer; }
                else { Main.Update -= flickerer; }
            }

            cabLight.gameObject.SetActive(IsOn);

            if (IsOn) { Main.Update += cabFlickerer; }
            else { Main.Update -= cabFlickerer; }
        }

        private Action<float> FlickerLight(Light light, float min = 0f, float max = 1f, float rate = 1f, float scale = 1f)
        {
            float t = 100f * (float)new System.Random().NextDouble();

            Action<float> flicker = delta =>
            {
                light.intensity = scale * ((max - min) * Mathf.PerlinNoise(t, 0f) + min);

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
                        new LocoLightData(LocoLightType.FHL, 4f, 1f, 2.25f)
                    }, new LocoLightData(LocoLightType.cab, 4f, 0.015f, 0.2f));

                    // Front Head Light
                    go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.name = LocoLightType.FHL.ToString();
                    GameObject.Destroy(go.GetComponent<BoxCollider>());
                    go.transform.position = car.transform.position;
                    go.transform.rotation = car.transform.rotation;
                    go.transform.localScale = new Vector3(0.28f, 0.28f, 0.05f);
                    r = go.GetComponent<Renderer>();
                    r.material.color = new Color32(255, 255, 255, 150);
                    StandardShaderUtils.ChangeRenderMode(r.material, StandardShaderUtils.BlendMode.Transparent);
                    r.enabled = false;
                    l = go.AddComponent<Light>();
                    l.shadows = LightShadows.Soft;
                    l.type = LightType.Spot;
                    l.spotAngle = 50;
                    l.color = new Color32(255, 251, 225, 255);
                    l.range = 26;
                    l.enabled = false;

                    go.transform.position += car.transform.forward * 10.96f;
                    go.transform.position += car.transform.up * 3.58f;

                    go.transform.SetParent(car.transform, true);

                    // default cab light: Color32(255, 253, 240, 255)
                    car.transform.Find("[cab light]").GetComponent<Light>().color = new Color32(255, 178, 63, 255);
                    break;

                case TrainCarType.LocoShunter:
                    car.transform.gameObject.AddComponent<LocoLights>();
                    car.transform.gameObject.GetComponent<LocoLights>().Init(car, new LocoLightData[]
                    {
                        new LocoLightData(LocoLightType.LFDL, 4f, 1f, 1.75f, 1f, 0.35f, 0.35f),
                        new LocoLightData(LocoLightType.RFDL, 4f, 1f, 1.75f, 1f, 0.35f, 0.35f),
                        new LocoLightData(LocoLightType.LRDL, 4f, 1f, 1.75f, 0.35f, 1f, 0.35f),
                        new LocoLightData(LocoLightType.RRDL, 4f, 1f, 1.75f, 0.35f, 1f, 0.35f)
                    }, new LocoLightData(LocoLightType.cab, 2f, 0.075f, 0.1f));

                    // Left Front Ditch Light
                    go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.name = LocoLightType.LFDL.ToString();
                    GameObject.Destroy(go.GetComponent<BoxCollider>());
                    go.transform.position = car.transform.position;
                    go.transform.rotation = car.transform.rotation;
                    go.transform.localScale = new Vector3(0.21f, 0.21f, 0.05f);
                    r = go.GetComponent<Renderer>();
                    r.material.color = new Color32(255, 255, 255, 150);
                    StandardShaderUtils.ChangeRenderMode(r.material, StandardShaderUtils.BlendMode.Transparent);
                    r.enabled = false;
                    l = go.AddComponent<Light>();
                    l.shadows = LightShadows.Soft;
                    l.type = LightType.Spot;
                    l.spotAngle = 50;
                    l.color = new Color32(255, 251, 225, 255);
                    l.range = 26;
                    l.enabled = false;

                    offsetX = 0.65f;
                    go.transform.position += car.transform.forward * 3.235f;
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
                    go = GameObject.Instantiate(go, car.transform.position, Quaternion.Euler(euler.x, euler.y + 180f, euler.z));
                    go.name = LocoLightType.LRDL.ToString();
                    (r = go.GetComponent<Renderer>()).enabled = false;
                    (l = go.GetComponent<Light>()).enabled = false;

                    offsetX = 0.65f;
                    go.transform.position += -car.transform.forward * 3.227f;
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
                        new LocoLightData(LocoLightType.FHL, 4f, 1f, 2f, 1f, 0.35f, 0.35f),
                        new LocoLightData(LocoLightType.LFDL, 4f, 1f, 2f, 1f, 0.35f, 0.35f),
                        new LocoLightData(LocoLightType.RFDL, 4f, 1f, 2f, 1f, 0.35f, 0.35f),
                        new LocoLightData(LocoLightType.RHL, 4f, 1f, 2f, 0.35f, 1f, 0.35f),
                        new LocoLightData(LocoLightType.LRDL, 4f, 1f, 2f, 0.35f, 1f, 0.35f),
                        new LocoLightData(LocoLightType.RRDL, 4f, 1f, 2f, 0.35f, 1f, 0.35f)
                    }, new LocoLightData(LocoLightType.cab, 2f, 0f, 0f)); // 0.075f, 0.1f));

                    // Front Head Light
                    go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.name = LocoLightType.FHL.ToString();
                    GameObject.Destroy(go.GetComponent<BoxCollider>());
                    go.transform.position = car.transform.position;
                    go.transform.rotation = Quaternion.Euler(euler.x + 7.5f, euler.y, euler.z);
                    go.transform.localScale = new Vector3(0.19f, 0.19f, 0.05f);
                    r = go.GetComponent<Renderer>();
                    r.material.color = new Color32(255, 255, 255, 150);
                    StandardShaderUtils.ChangeRenderMode(r.material, StandardShaderUtils.BlendMode.Transparent);
                    r.enabled = false;
                    l = go.AddComponent<Light>();
                    l.shadows = LightShadows.Soft;
                    l.type = LightType.Spot;
                    l.spotAngle = 28;
                    l.color = new Color32(255, 251, 225, 255);
                    l.range = 98;
                    l.enabled = false;

                    offsetY = 3.765f;
                    offsetZ = 7.168f;
                    go.transform.position += car.transform.forward * offsetZ;
                    go.transform.position += car.transform.up * offsetY;

                    go.transform.SetParent(car.transform, true);

                    // Left Front Ditch Light
                    go = GameObject.Instantiate(go, go.transform.position, go.transform.rotation);
                    go.name = LocoLightType.LFDL.ToString();
                    (r = go.GetComponent<Renderer>()).enabled = false;
                    (l = go.GetComponent<Light>()).enabled = false;
                    l.spotAngle = 35;
                    l.range = 26;

                    offsetX = 0.6115f;
                    offsetθ = 13.5f;
                    go.transform.position += car.transform.forward * (8.368f - offsetZ);
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
                    go = GameObject.Instantiate(go, car.transform.position, Quaternion.Euler(euler.x + 7.5f, euler.y + 180f, euler.z));
                    go.name = LocoLightType.RHL.ToString();
                    (r = go.GetComponent<Renderer>()).enabled = false;
                    (l = go.GetComponent<Light>()).enabled = false;
                    l.spotAngle = 28;
                    l.range = 98;

                    offsetY = 3.43f;
                    offsetZ = 8.27f;
                    go.transform.position += -car.transform.forward * offsetZ;
                    go.transform.position += car.transform.up * offsetY;
                    go.transform.localScale = new Vector3(0.21f, 0.21f, 0.05f);

                    go.transform.SetParent(car.transform, true);

                    // Left Rear Ditch Light
                    go = GameObject.Instantiate(go, go.transform.position, go.transform.rotation);
                    go.name = LocoLightType.LRDL.ToString();
                    (r = go.GetComponent<Renderer>()).enabled = false;
                    (l = go.GetComponent<Light>()).enabled = false;
                    l.spotAngle = 35;
                    l.range = 26;

                    offsetX = 0.615f;
                    go.transform.position += -car.transform.forward * (8.237f - offsetZ);
                    go.transform.position += car.transform.up * (2.007f - offsetY);
                    go.transform.position += -car.transform.right * offsetX;
                    go.transform.localScale = new Vector3(0.165f, 0.165f, 0.05f);

                    go.transform.SetParent(car.transform, true);

                    // Right Rear Ditch Light
                    go = GameObject.Instantiate(go, go.transform.position, go.transform.rotation);
                    go.name = LocoLightType.RRDL.ToString();
                    (r = go.GetComponent<Renderer>()).enabled = false;
                    (l = go.GetComponent<Light>()).enabled = false;

                    go.transform.position += car.transform.right * 2f * offsetX;

                    go.transform.SetParent(car.transform, true);

                    // default cab light: Color32(255, 253, 240, 255)
                    car.transform.Find("[cab light]").GetComponent<Light>().color = new Color(255, 251, 225, 255);
                    break;
            }
        }
    }
}
