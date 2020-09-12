using UnityEngine;

namespace LocoLightsMod.LocoLightDefinitions
{
    internal class DE6LightDefinition
    {
        static readonly string[] extLights = new string[]
        {
            "FHL",
            "RHL",
            "LFDL",
            "RFDL",
            "LRDL",
            "RRDL",
            "FWL",
            "RWL",
        };
        static readonly string[] intLights = new string[]
        {
            "[cab light override]",
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
                    new LocoLightData(extLights[0], 4f, 1.25f, 2f, 1f, 0f, 0.1f),
                    new LocoLightData(extLights[1], 4f, 1.25f, 2f, 0f, 1f, 0.1f),
                    new LocoLightData(extLights[2], 4f, 1f, 1.75f, 1f, 0f, 0.1f),
                    new LocoLightData(extLights[3], 4f, 1f, 1.75f, 1f, 0f, 0.1f),
                    new LocoLightData(extLights[4], 4f, 1f, 1.75f, 0f, 1f, 0.1f),
                    new LocoLightData(extLights[5], 4f, 1f, 1.75f, 0f, 1f, 0.1f),
                    new LocoLightData(extLights[6], 3f, 0.5f, 0.6f, 0f, 1f, 0f),
                    new LocoLightData(extLights[7], 3f, 0.5f, 0.6f, 1f, 0f, 0f),
                },
                new LocoLightData[] {
                    new LocoLightData(intLights[0], 2f, 0.375f, 0.5f),
                });

            // make the red lenses transparent
            foreach (Renderer rend in car.transform.GetComponentsInChildren<Renderer>())
            {
                foreach (Material mat in rend.materials)
                {
                    if (mat.name == "headlights_red (Instance)")
                    {
                        StandardShaderUtils.ChangeRenderMode(mat, StandardShaderUtils.BlendMode.Transparent);
                    }
                }
            }
        }

        private static void CreateLights(TrainCar car)
        {
            GameObject go;
            Renderer r;
            Light l;

            float offsetX;
            float offsetY;
            float offsetZ;
            float offsetθ;
            Vector3 euler = car.transform.rotation.eulerAngles;

            // Front Head Light
            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = extLights[0];
            GameObject.DestroyImmediate(go.GetComponent<Collider>());
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
            l.color = new Color32(255, 251, 225, 0);
            l.range = 98;
            l.enabled = false;

            offsetY = 3.765f;
            offsetZ = 7.128f;
            go.transform.position += car.transform.forward * offsetZ;
            go.transform.position += car.transform.up * offsetY;

            go.transform.SetParent(car.transform, true);

            // Left Front Ditch Light
            go = GameObject.Instantiate(go, go.transform.position, go.transform.rotation);
            go.name = extLights[2];
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
            go.name = extLights[3];
            (r = go.GetComponent<Renderer>()).enabled = false;
            (l = go.GetComponent<Light>()).enabled = false;

            go.transform.position += car.transform.right * 2f * offsetX;
            go.transform.rotation = Quaternion.Euler(euler.x + 7.5f, euler.y + offsetθ, euler.z);

            go.transform.SetParent(car.transform, true);

            // Rear Head Light
            go = GameObject.Instantiate(go, car.transform.position, Quaternion.Euler(euler.x + 10f, euler.y + 180f, euler.z));
            go.name = extLights[1];
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
            go.name = extLights[4];
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
            go.name = extLights[5];
            (r = go.GetComponent<Renderer>()).enabled = false;
            (l = go.GetComponent<Light>()).enabled = false;

            go.transform.position += car.transform.right * 2f * offsetX;
            go.transform.rotation = Quaternion.Euler(euler.x + 7.5f, euler.y + 180f - offsetθ, euler.z);

            go.transform.SetParent(car.transform, true);

            // Rear Warning Light
            go = GameObject.Instantiate(go, car.transform.position, Quaternion.Euler(euler.x, euler.y + 180f, euler.z));
            go.name = extLights[7];
            (r = go.GetComponent<Renderer>()).enabled = false;
            (l = go.GetComponent<Light>()).enabled = false;
            l.color = new Color32(255, 64, 64, 0);
            l.innerSpotAngle = 32;
            l.spotAngle = 64;
            l.range = 26;
            r.material.color = l.color;

            go.transform.position += -car.transform.forward * offsetZ;
            go.transform.position += car.transform.up * 3.117f;
            go.transform.localScale = new Vector3(0.21f, 0.21f, 0.05f);

            go.transform.SetParent(car.transform, true);

            // Front Warning Light
            go = GameObject.Instantiate(go, car.transform.position, car.transform.rotation);
            go.name = extLights[6];
            (r = go.GetComponent<Renderer>()).enabled = false;
            (l = go.GetComponent<Light>()).enabled = false;

            go.transform.position += car.transform.forward * 8.448f;
            go.transform.position += car.transform.up * 2.83f;
            go.transform.localScale = new Vector3(0.21f, 0.21f, 0.05f);

            go.transform.SetParent(car.transform, true);

            // this is dumb, but DE6 cab light is fucked
            Transform cabLight = car.transform.Find("[cab light]");
            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = intLights[0];
            r = go.GetComponent<Renderer>();
            r.enabled = false;
            l = go.AddComponent<Light>();
            l.type = LightType.Point;
            l.shadows = LightShadows.Hard;
            // default cab light: Color32(255, 253, 240, 255)
            l.color = new Color32(255, 251, 225, 255);
            l.range = 6.6f;
            l.enabled = false;
            go.transform.position = cabLight.position - 0.5f * car.transform.up;
            go.transform.SetParent(car.transform, true);
        }
    }
}
