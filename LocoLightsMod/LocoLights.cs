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

        public void Init(TrainCar car, LocoLightData[] exterior, LocoLightData cab)
		{
            Debug.Log("init train car " + car.logicCar.ID);
            this.car = car;
            this.exterior = exterior;
            this.cab = cab;
            this.cabLight = car.transform.Find("[cab light]");
        }

        public void ToggleLights()
        {
            if (car == null || exterior == null || cabLight == null) { return; }
            Debug.Log("toggle train car " + car.logicCar.ID);

            if (renderers == null)
            {
                renderers = new Renderer[exterior.Length];
                lights = new Light[exterior.Length];
                flickerers = new Action<float>[exterior.Length];
                cabFlickerer = FlickerLight(cabLight.GetComponent<Light>(), cab.min, cab.max, cab.flicker);

                for (int i = 0; i < exterior.Length; i++)
				{
                    LocoLightData datum = exterior[i];
                    renderers[i] = car.transform.Find(datum.type.ToString()).gameObject.GetComponent<Renderer>();
                    lights[i] = renderers[i].transform.gameObject.GetComponent<Light>();
                    flickerers[i] = FlickerLight(lights[i], datum.min, datum.max, datum.flicker);
				}
            }

            IsOn = !IsOn;

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

        private Action<float> FlickerLight(Light light, float min = 0f, float max = 1f, float scale = 1f, int factor = 1)
        {
            float t = 0;
            factor = Math.Max(factor, 1);

            // prevent flash bulb effect when first turning on
            light.intensity = 0f;

            return delta =>
            {
                float multiplier = 1f;
                for (int i = 0; i < factor; i++) { multiplier *= Mathf.PerlinNoise(t, 0f); }
                light.intensity = (max - min) * multiplier + min;

                t += delta * scale;
            };
        }

        public static void SetupLights(TrainCar c)
        {
            TrainCar car = c;
            if (car == null) { return; }

            // Front Head Light
            GameObject FHL;
            Renderer FHL_R;
            Light FHL_Light;

            // Left Front Ditch Light
            GameObject LFDL;
            Renderer LFDL_R;
            Light LFDL_Light;

            // Right Front Ditch Light
            GameObject RFDL;
            Renderer RFDL_R;
            Light RFDL_Light;

            // Rear Head Light
            GameObject RHL;
            Renderer RHL_R;
            Light RHL_Light;

            // Left Rear Ditch Light
            GameObject LRDL;
            Renderer LRDL_R;
            Light LRDL_Light;

            // Right Rear Ditch Light
            GameObject RRDL;
            Renderer RRDL_R;
            Light RRDL_Light;

            float FDL_Offset;
            float RDL_Offset;
            Vector3 euler;

            switch (car.carType)
            {
                case TrainCarType.LocoSteamHeavy:
                    car.transform.gameObject.AddComponent<LocoLights>();
                    car.transform.gameObject.GetComponent<LocoLights>().Init(car, new LocoLightData[]
                    {
                        new LocoLightData(LocoLightType.FHL, 4f, 1f, 2.25f)
                    }, new LocoLightData(LocoLightType.cab, 4f, 0.015f, 0.2f));

                    FHL = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    FHL.name = LocoLightType.FHL.ToString();
                    GameObject.Destroy(FHL.GetComponent<BoxCollider>());
                    FHL.transform.position = car.transform.position;
                    FHL.transform.rotation = car.transform.rotation;
                    FHL.transform.localScale = new Vector3(0.28f, 0.28f, 0.05f);
                    FHL_R = FHL.GetComponent<Renderer>();
                    FHL_R.material.color = new Color32(255, 255, 255, 150);
                    StandardShaderUtils.ChangeRenderMode(FHL_R.material, StandardShaderUtils.BlendMode.Transparent);
                    FHL_R.enabled = false;

                    FHL.transform.position += car.transform.forward * 10.96f;
                    FHL.transform.position += car.transform.up * 3.58f;

                    FHL_Light = FHL.AddComponent<Light>();
                    FHL_Light.shadows = LightShadows.Soft;
                    FHL_Light.type = LightType.Spot;
                    FHL_Light.spotAngle = 50;
                    FHL_Light.color = new Color32(255, 251, 225, 255);
                    FHL_Light.range = 26;
                    FHL_Light.intensity = 2.25f;
                    FHL_Light.enabled = false;

                    FHL.transform.SetParent(car.transform, true);

                    // default cab light: Color32(255, 253, 240, 255)
                    car.transform.Find("[cab light]").GetComponent<Light>().color = new Color32(255, 178, 63, 255);
                    break;

                case TrainCarType.LocoShunter:
                    car.transform.gameObject.AddComponent<LocoLights>();
                    car.transform.gameObject.GetComponent<LocoLights>().Init(car, new LocoLightData[]
                    {
                        new LocoLightData(LocoLightType.LFDL, 4f, 1f, 1.75f, 1f, 0.3f, 0.3f),
                        new LocoLightData(LocoLightType.RFDL, 4f, 1f, 1.75f, 1f, 0.3f, 0.3f),
                        new LocoLightData(LocoLightType.LRDL, 4f, 1f, 1.75f, 0.3f, 1f, 0.3f),
                        new LocoLightData(LocoLightType.RRDL, 4f, 1f, 1.75f, 0.3f, 1f, 0.3f)
                    }, new LocoLightData(LocoLightType.cab, 4f, 0.05f, 0.1f));

                    LFDL = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    LFDL.name = LocoLightType.LFDL.ToString();
                    GameObject.Destroy(LFDL.GetComponent<BoxCollider>());
                    LFDL.transform.position = car.transform.position;
                    LFDL.transform.rotation = car.transform.rotation;
                    LFDL.transform.localScale = new Vector3(0.21f, 0.21f, 0.05f);
                    LFDL_R = LFDL.GetComponent<Renderer>();
                    LFDL_R.material.color = new Color32(255, 255, 255, 150);
                    StandardShaderUtils.ChangeRenderMode(LFDL_R.material, StandardShaderUtils.BlendMode.Transparent);
                    LFDL_R.enabled = false;

                    FDL_Offset = 0.65f;
                    LFDL.transform.position += car.transform.forward * 3.235f;
                    LFDL.transform.position += car.transform.up * 1.528f;
                    LFDL.transform.position += -car.transform.right * FDL_Offset;

                    LFDL_Light = LFDL.AddComponent<Light>();
                    LFDL_Light.shadows = LightShadows.Soft;
                    LFDL_Light.type = LightType.Spot;
                    LFDL_Light.spotAngle = 50;
                    LFDL_Light.color = new Color32(255, 251, 225, 255);
                    LFDL_Light.range = 26;
                    LFDL_Light.intensity = 2.25f;
                    LFDL_Light.enabled = false;

                    RFDL = GameObject.Instantiate(LFDL, LFDL.transform.position, LFDL.transform.rotation);
                    RFDL.name = LocoLightType.RFDL.ToString();
                    (RFDL_R = RFDL.GetComponent<Renderer>()).enabled = false;
                    (RFDL_Light = RFDL.GetComponent<Light>()).enabled = false;
                    RFDL.transform.position += car.transform.right * 2f * FDL_Offset;

                    RDL_Offset = 0.65f;
                    euler = car.transform.rotation.eulerAngles;
                    LRDL = GameObject.Instantiate(LFDL, car.transform.position, Quaternion.Euler(euler.x, euler.y + 180f, euler.z));
                    LRDL.name = LocoLightType.LRDL.ToString();
                    (LRDL_R = LRDL.GetComponent<Renderer>()).enabled = false;
                    (LRDL_Light = LRDL.GetComponent<Light>()).enabled = false;
                    LRDL.transform.position += -car.transform.forward * 3.227f;
                    LRDL.transform.position += car.transform.up * 1.928f;
                    LRDL.transform.position += -car.transform.right * RDL_Offset;

                    RRDL = GameObject.Instantiate(LRDL, LRDL.transform.position, LRDL.transform.rotation);
                    RRDL.name = LocoLightType.RRDL.ToString();
                    (RRDL_R = RRDL.GetComponent<Renderer>()).enabled = false;
                    (RRDL_Light = RRDL.GetComponent<Light>()).enabled = false;
                    RRDL.transform.position += car.transform.right * 2f * RDL_Offset;

                    LFDL.transform.SetParent(car.transform, true);
                    RFDL.transform.SetParent(car.transform, true);
                    LRDL.transform.SetParent(car.transform, true);
                    RRDL.transform.SetParent(car.transform, true);

                    // default cab light: Color32(255, 253, 240, 255)
                    car.transform.Find("[cab light]").GetComponent<Light>().color = new Color32(255, 251, 225, 255);
                    break;

                case TrainCarType.LocoDiesel:
                    // default cab light: Color32(255, 253, 240, 255)
                    car.transform.Find("[cab light]").GetComponent<Light>().color = new Color(255, 251, 225, 255);
                    break;
            }
        }
    }
}
