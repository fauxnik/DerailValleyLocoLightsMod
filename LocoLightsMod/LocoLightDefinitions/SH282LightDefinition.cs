using UnityEngine;

namespace LocoLightsMod.LocoLightDefinitions
{
    internal class SH282LightDefinition
    {
        public static void SetupLights(TrainCar car)
        {
            if (car == null) { return; }

            GameObject go;
            Renderer r;
            Light l;

            Vector3 euler = car.transform.rotation.eulerAngles;

            car.transform.gameObject.AddComponent<LocoLights>();
            car.transform.gameObject.GetComponent<LocoLights>().Init(
                car,
                new LocoLightData[]
                {
                    new LocoLightData(LocoLightType.FHL, 4f, 0.75f, 1.75f)
                },
                new LocoLightData[] {
                    new LocoLightData(LocoLightType.cab, 5f, 0.015f, 0.25f)
                });

            // Front Head Light
            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = LocoLightType.FHL.ToString();
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
            car.transform.Find("[cab light]").GetComponent<Light>().color = new Color32(255, 179, 63, 255);
        }
    }
}
