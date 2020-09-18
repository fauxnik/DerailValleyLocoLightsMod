using UnityEngine;

namespace LocoLightsMod.LocoLightDefinitions
{
    internal class SH282LightDefinition
    {
        static readonly string[] extLights = new string[]
        {
            "FHL",
            "RHL",
        };
        static readonly string[] intLights = new string[]
        {
            "[cab light]",
        };

        public static void SetupEngine(TrainCar engine)
        {
            if (engine == null) { return; }

            engine.rearCoupler.Coupled += (sender, args) =>
            {
                SetupTender(args.otherCoupler.train, args.thisCoupler.train);
            };
            engine.rearCoupler.Uncoupled += (sender, args) =>
            {
                TeardownTender(args.otherCoupler.train, args.thisCoupler.train);
            };

            CreateEngineLights(engine);

            engine.transform.gameObject.AddComponent<LocoLights>();
            engine.transform.gameObject.GetComponent<LocoLights>().Init(
                engine,
                new LocoLightData[]
                {
                    new LocoLightData(extLights[0], 4f, 0.75f, 1.75f, 1f, 0f, 0.1f),
                },
                new LocoLightData[] {
                    new LocoLightData(intLights[0], 5f, 0.015f, 0.25f),
                });
        }

        public static void SetupTender(TrainCar tender, TrainCar engine)
        {
            if (tender == null) { return; }

            CreateTenderLights(tender, engine);

            engine.gameObject.GetComponent<LocoLights>().AddExtLights(
                tender,
                new LocoLightData[]
                {
                    new LocoLightData(extLights[1], 4f, 0.75f, 1.75f, 0f, 1f, 0.1f),
                });
        }

        public static void TeardownTender(TrainCar tender, TrainCar engine)
        {
            if (tender == null) { return; }

            engine.gameObject.GetComponent<LocoLights>().RemoveExtLight(extLights[1]);

            var go = GameObjectUtils.FindObject(tender.gameObject, "tender headlight");
            Main.Log($"deleting {go?.name}...");
            GameObject.Destroy(go);
        }

        private static void CreateEngineLights(TrainCar engine)
        {
            Main.Log("creating engine lights...");
            GameObject go;
            Renderer r;
            Light l;

            Vector3 euler = engine.transform.rotation.eulerAngles;

            // Front Head Light
            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = extLights[0];
            GameObject.DestroyImmediate(go.GetComponent<Collider>());
            go.transform.position = engine.transform.position;
            go.transform.rotation = Quaternion.Euler(euler.x + 10f, euler.y, euler.z);
            go.transform.localScale = new Vector3(0.28f, 0.28f, 0.05f);
            r = go.GetComponent<Renderer>();
            r.material.color = new Color32(255, 255, 255, 0);
            StandardShaderUtils.ChangeRenderMode(r.material, StandardShaderUtils.BlendMode.Emission);
            r.enabled = false;
            l = go.AddComponent<Light>();
            l.shadows = Main.settings.exteriorShadows;
            l.type = LightType.Spot;
            l.innerSpotAngle = 14;
            l.spotAngle = 42;
            l.color = new Color32(255, 198, 111, 0);
            l.range = 98;
            l.enabled = false;

            go.transform.position += engine.transform.forward * 10.86f;
            go.transform.position += engine.transform.up * 3.58f;

            go.transform.SetParent(engine.transform, true);

            // default cab light: Color32(255, 253, 240, 255)
            go = engine.transform.Find("[cab light]").gameObject;
            go.SetActive(true);
            l = go.GetComponent<Light>();
            l.shadows = Main.settings.interiorShadows;
            l.color = new Color32(255, 179, 63, 255);
            l.enabled = false;
        }

        private static void CreateTenderLights(TrainCar tender, TrainCar engine)
        {
            Main.Log("creating tender lights...");
            GameObject go;
            Renderer r;
            Light l;

            // Rear Head Light
            go = GameObject.Instantiate(Main.assets["SH282_Tender_Light_Body"], tender.transform.position, tender.transform.rotation, tender.transform);
            go.name = "tender headlight";
            go.transform.Find("Body").GetComponent<MeshRenderer>().sharedMaterial = engine.transform.Find("Exterior").Find("SH_exterior").Find("ext Locomotive Body").GetComponent<MeshRenderer>().sharedMaterial;
            go.transform.Find("Bulb").GetComponent<MeshRenderer>().sharedMaterial = engine.transform.Find("Exterior").Find("SH_exterior").Find("ext Headlight Lightbulb").GetComponent<MeshRenderer>().sharedMaterial;
            go.transform.Find("Glass").GetComponent<MeshRenderer>().sharedMaterial = engine.transform.Find("Exterior").Find("SH_exterior").Find("ext Headlight Glass").GetComponent<MeshRenderer>().sharedMaterial;
            go = GameObjectUtils.FindObject(go, "Light Disk");
            go.name = extLights[1];
            //go.transform.SetParent(tender.transform, true);
            GameObject.DestroyImmediate(go.GetComponent<Collider>());
            r = go.GetComponent<Renderer>();
            r.material.color = new Color32(255, 255, 255, 0);
            StandardShaderUtils.ChangeRenderMode(r.material, StandardShaderUtils.BlendMode.Emission);
            r.enabled = false;
            l = go.AddComponent<Light>();
            l.shadows = Main.settings.exteriorShadows;
            l.type = LightType.Spot;
            l.innerSpotAngle = 14;
            l.spotAngle = 42;
            l.color = new Color32(255, 198, 111, 0);
            l.range = 98;
            l.enabled = false;
        }
    }
}
