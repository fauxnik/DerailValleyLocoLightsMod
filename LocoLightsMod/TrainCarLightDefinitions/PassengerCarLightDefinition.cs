using UnityEngine;

namespace LocoLightsMod.TrainCarLightDefinitions
{
    class PassengerCarLightDefinition
    {

        static readonly string[] extLights = new string[]
        {
        };
        static readonly string[] intLights = new string[]
        {
                "[int light 1]",
                "[int light 2]",
                "[int light 3]",
                "[int light 4]",
                "[int light 5]",
                "[int light 6]",
                "[int light 7]",
                "[int light 8]",
                "[int light 9]",
                "[int light 10]",
                "[int light 11]",
        };

        public static void SetupLights(TrainCar car)
        {
            if (car == null) { return; }

            CreateLights(car);

            car.transform.gameObject.AddComponent<TrainCarLights>();
            car.transform.gameObject.GetComponent<TrainCarLights>().Init(
                car,
                new LocoLightData[] { },
                new LocoLightData[] {
                    new LocoLightData(intLights[0], 0f, 0.2f, 0.2f),
                    new LocoLightData(intLights[1], 0f, 0.2f, 0.2f),
                    new LocoLightData(intLights[2], 0f, 0.2f, 0.2f),
                    new LocoLightData(intLights[3], 0f, 0.2f, 0.2f),
                    new LocoLightData(intLights[4], 0f, 0.2f, 0.2f),
                    new LocoLightData(intLights[5], 0f, 0.2f, 0.2f),
                    new LocoLightData(intLights[6], 0f, 0.2f, 0.2f),
                    new LocoLightData(intLights[7], 0f, 0.2f, 0.2f),
                    new LocoLightData(intLights[8], 0f, 0.2f, 0.2f),
                    new LocoLightData(intLights[9], 0f, 0.2f, 0.2f),
                    new LocoLightData(intLights[10], 0f, 0.2f, 0.2f),
                });
        }

        private static void CreateLights(TrainCar car)
        {
            GameObject go;
            Renderer r;
            Light l;

            Vector3 euler = car.transform.rotation.eulerAngles;

            Main.Log($"Creating light {intLights[0]}");
            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = intLights[0];
            GameObject.DestroyImmediate(go.GetComponent<Collider>());
            r = go.GetComponent<Renderer>();
            r.enabled = false;
            l = go.AddComponent<Light>();
            l.type = LightType.Point;
            l.shadows = LightShadows.Hard;
            l.color = new Color32(255, 251, 225, 255);
            l.range = 6.6f;
            l.enabled = true;
            go.transform.SetParent(car.transform, false);
            go.transform.localPosition = 3.6f * Vector3.up - 8f * Vector3.forward;
            go.transform.localScale = 0.25f * Vector3.one;

            for (int i = 1; i < 9; i++)
            {
                Main.Log($"Creating light {intLights[i]}");
                go = GameObject.Instantiate(go, go.transform.position, car.transform.rotation, car.transform);
                go.name = intLights[i];
                go.transform.localPosition += 2f * Vector3.forward;
            }

            Main.Log($"Creating light {intLights[9]}");
            go = GameObject.Instantiate(go, car.transform.position, car.transform.rotation, car.transform);
            go.name = intLights[9];
            go.transform.localPosition = 3.6f * Vector3.up - 10.6f * Vector3.forward;

            Main.Log($"Creating light {intLights[10]}");
            go = GameObject.Instantiate(go, car.transform.position, car.transform.rotation, car.transform);
            go.name = intLights[10];
            go.transform.localPosition += 3.6f * Vector3.up + 10.6f * Vector3.forward;
        }
    }
}
