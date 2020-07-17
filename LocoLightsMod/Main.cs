using Harmony12;
using System;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace LocoLightsMod
{
    public class Main
    {
        public static bool enabled;
        public static Action<float> Update;

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;

            return true;
        }

        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            modEntry.OnToggle = OnToggle;
            modEntry.OnUpdate = OnUpdate;

            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            return true;
        }

        private static void OnUpdate(UnityModManager.ModEntry modEntry, float delta)
        {
            if (!enabled) { return; }

            if (Update != null)
			{
                Update(delta);
			}
        }

        [HarmonyPatch(typeof(TrainCar), "Start")]
        internal class LocoSteamHeavyPatch
        {
            private static void Postfix(TrainCar __instance)
            {
                if (!Main.enabled) return;

                try
                {
                    Main.SetupLights(__instance);
                }
                catch { }
            }
        }

        public static void SetupLights(TrainCar c)
        {
            TrainCar car = c;
            if (car == null) { return; }

            GameObject FHL;
            Renderer FHL_R;
            Light FHL_Light;

            switch (car.carType)
            {
            case TrainCarType.LocoSteamHeavy:
                FHL = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                FHL.name = "FHL";
                GameObject.Destroy(FHL.GetComponent<BoxCollider>());
                FHL.AddComponent<LocoLights>();
                FHL.transform.position = car.transform.position;
                FHL.transform.rotation = car.transform.rotation;
                FHL.transform.localScale = new Vector3(0.28f, 0.28f, 0.05f);
                FHL_R = FHL.GetComponent<Renderer>();
                StandardShaderUtils.ChangeRenderMode(FHL_R.material, StandardShaderUtils.BlendMode.Transparent);
                FHL_R.material.color = new Color32(255, 255, 255, 150);
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
                // default cab light: Color32(255, 253, 240, 255)
                car.transform.Find("[cab light]").GetComponent<Light>().color = new Color32(255, 251, 225, 255);
                break;

            case TrainCarType.LocoDiesel:
                // default cab light: Color32(255, 253, 240, 255)
                car.transform.Find("[cab light]").GetComponent<Light>().color = new Color(255, 251, 225, 255);
                break;
            }
        }

        public static TrainCar FindClosestTrainCar()
        {
            Camera main = Camera.main;
            float num = float.PositiveInfinity;
            TrainCar car1 = null;
            foreach (TrainCar car2 in UnityEngine.Object.FindObjectsOfType<TrainCar>())
            {
                float sqrMagnitude = (main.transform.position - car2.transform.position).sqrMagnitude;
                if (sqrMagnitude <= (double)num)
                {
                    num = sqrMagnitude;
                    car1 = car2;
                }
            }
            return car1;
        }
    }
}
