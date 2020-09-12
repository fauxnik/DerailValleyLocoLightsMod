using UnityEngine;

namespace LocoLightsMod.TrainCarLightDefinitions
{
    class CabooseLightDefinitions
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
                    new LocoLightData(intLights[0], 0f, 0.1f, 0.1f),
                    new LocoLightData(intLights[1], 0f, 0.1f, 0.1f),
                    new LocoLightData(intLights[2], 0f, 0.1f, 0.1f),
                    new LocoLightData(intLights[3], 0f, 0.1f, 0.1f),
                });
        }

        private static void CreateLights(TrainCar car)
        {
            GameObject go;
            Renderer r;
            Light l;

            Vector3 euler = car.transform.rotation.eulerAngles;

            Main.Log($"Creating light {intLights[0]}");
            go = new GameObject() { name = intLights[0] };
            l = go.AddComponent<Light>();
            l.type = LightType.Point;
            l.shadows = LightShadows.Hard;
            l.color = new Color32(255, 198, 111, 255);
            l.range = 6.6f;
            l.enabled = true;
            go.transform.SetParent(car.transform, false);
            go.transform.localPosition = 4.1f * Vector3.up - 1.25f * Vector3.forward;
            go.transform.localScale = 0.25f * Vector3.one;

            Main.Log($"Creating light {intLights[1]}");
            go = GameObject.Instantiate(go, car.transform.position, car.transform.rotation, car.transform);
            go.name = intLights[1];
            go.transform.localPosition = 3.2f * Vector3.up - 3.75f * Vector3.forward;

            Main.Log($"Creating light {intLights[2]}");
            go = GameObject.Instantiate(go, car.transform.position, car.transform.rotation, car.transform);
            go.name = intLights[2];
            go.transform.localPosition += 3.2f * Vector3.up + 1.25f * Vector3.forward;

            Main.Log($"Creating light {intLights[3]}");
            go = GameObject.Instantiate(go, car.transform.position, car.transform.rotation, car.transform);
            go.name = intLights[3];
            go.transform.localPosition = 3.2f * Vector3.up + 3.75f * Vector3.forward;
        }
    }
}
