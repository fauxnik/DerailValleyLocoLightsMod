using UnityEngine;

namespace LocoLightsMod.LocoLightDefinitions
{
    internal class DE2LightDefinition
    {
        public static void SetupLights(TrainCar car)
        {
            if (car == null) { return; }

            GameObject go;
            Renderer r;
            Light l;

            float offsetX;
            Vector3 euler = car.transform.rotation.eulerAngles;

            car.transform.gameObject.AddComponent<LocoLights>();
            car.transform.gameObject.GetComponent<LocoLights>().Init(
                car,
                new LocoLightData[]
                {
                    new LocoLightData(LocoLightType.LFDL, 4f, 1f, 1.75f, 1f, 0f, 0.1f),
                    new LocoLightData(LocoLightType.RFDL, 4f, 1f, 1.75f, 1f, 0f, 0.1f),
                    new LocoLightData(LocoLightType.LRDL, 4f, 1f, 1.75f, 0f, 1f, 0.1f),
                    new LocoLightData(LocoLightType.RRDL, 4f, 1f, 1.75f, 0f, 1f, 0.1f),
                    new LocoLightData(LocoLightType.FWL, 3f, 0.5f, 0.6f, 0f, 1f, 0f),
                    new LocoLightData(LocoLightType.RWL, 3f, 0.5f, 0.6f, 1f, 0f, 0f)
                },
                new LocoLightData[] {
                    new LocoLightData(LocoLightType.cab, 2f, 0.1f, 0.15f)
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

            // Left Front Ditch Light
            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = LocoLightType.LFDL.ToString();
            GameObject.DestroyImmediate(go.GetComponent<Collider>());
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
            l.color = new Color32(255, 251, 225, 0);
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

            // Rear Warning Light
            go = GameObject.Instantiate(go, car.transform.position, Quaternion.Euler(euler.x, euler.y + 180f, euler.z));
            go.name = LocoLightType.RWL.ToString();
            (r = go.GetComponent<Renderer>()).enabled = false;
            (l = go.GetComponent<Light>()).enabled = false;
            l.color = new Color32(255, 64, 64, 0);
            l.innerSpotAngle = 32;
            l.spotAngle = 64;
            l.range = 26;
            r.material.color = l.color;

            go.transform.position += -car.transform.forward * 3.167f;
            go.transform.position += car.transform.up * 2.47f;

            go.transform.SetParent(car.transform, true);

            // Front Warning Light
            go = GameObject.Instantiate(go, car.transform.position, car.transform.rotation);
            go.name = LocoLightType.FWL.ToString();
            (r = go.GetComponent<Renderer>()).enabled = false;
            (l = go.GetComponent<Light>()).enabled = false;

            go.transform.position += car.transform.forward * 3.135f;
            go.transform.position += car.transform.up * 2.82f;

            go.transform.SetParent(car.transform, true);

            // default cab light: Color32(255, 253, 240, 255)
            car.transform.Find("[cab light]").GetComponent<Light>().color = new Color32(255, 251, 225, 255);
        }
    }
}
