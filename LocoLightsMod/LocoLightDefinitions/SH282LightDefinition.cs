using UnityEngine;

namespace LocoLightsMod.LocoLightDefinitions
{
    internal class SH282LightDefinition
    {
        static readonly string[] extLights = new string[]
        {
            "FHL",
        };
        static readonly string[] intLights = new string[]
        {
            "[cab light]",
        };

        public static void SetupLights(TrainCar car)
        {
            if (car == null) { return; }

            CreateLights(car);

            car.transform.gameObject.AddComponent<LocoLights>();
            car.transform.gameObject.GetComponent<LocoLights>().Init(
                car,
                new LocoLightData[]
                {
                    new LocoLightData(extLights[0], 4f, 0.75f, 1.75f),
                },
                new LocoLightData[] {
                    new LocoLightData(intLights[0], 5f, 0.015f, 0.25f),
                });
        }

        private static void CreateLights(TrainCar car)
        {
            GameObject go;
            Renderer r;
            Light l;

            Vector3 euler = car.transform.rotation.eulerAngles;

            // Front Head Light
            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = extLights[0];
            GameObject.DestroyImmediate(go.GetComponent<Collider>());
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
            l.color = new Color32(255, 198, 111, 0);
            l.range = 98;
            l.enabled = false;

            go.transform.position += car.transform.forward * 10.86f;
            go.transform.position += car.transform.up * 3.58f;

            go.transform.SetParent(car.transform, true);

            // default cab light: Color32(255, 253, 240, 255)
            go = car.transform.Find("[cab light]").gameObject;
            go.SetActive(true);
            l = go.GetComponent<Light>();
            l.shadows = LightShadows.Hard;
            l.color = new Color32(255, 179, 63, 255);
            l.enabled = false;
        }
    }
}
