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

        private static void OnUpdate(UnityModManager.ModEntry modEntry, float tick)
        {
            if (!enabled) { return; }
        }

        [HarmonyPatch(typeof(TrainCar), "Start")]
        internal class LocoSteamHeavyPatch
        {
            private static void Postfix(TrainCar __instance)
            {
                if (!Main.enabled) return;

                try
                {
                    Main.CreateHeadLight(__instance);
                }
                catch { }
            }
        }

        public static void CreateHeadLight(TrainCar c)
        {
            TrainCar car = c;
            if (car == null) { return; }

            switch (car.carType)
            {
            case TrainCarType.LocoSteamHeavy:
                GameObject LDL = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                LDL.name = "LDL";
                GameObject.Destroy(LDL.GetComponent<BoxCollider>());
                LDL.AddComponent<LocoLights>();
                LDL.transform.position = car.transform.position;
                LDL.transform.rotation = car.transform.rotation;
                LDL.transform.localScale = new Vector3(0.28f, 0.28f, 0.05f);
                Renderer LDR = LDL.GetComponent<Renderer>();
                StandardShaderUtils.ChangeRenderMode(LDR.material, StandardShaderUtils.BlendMode.Transparent);
                LDR.material.color = new Color32(255, 255, 255, 150);
                LDR.enabled = false;

                LDL.transform.position += car.transform.forward * 10.96f;
                LDL.transform.position += car.transform.up * 3.58f;

                Light LDL_Light = LDL.AddComponent<Light>();
                LDL_Light.shadows = LightShadows.Soft;
                LDL_Light.type = LightType.Spot;
                LDL_Light.spotAngle = 50;
                LDL_Light.color = new Color32(255, 255, 255, 255);
                LDL_Light.range = 26;
                LDL_Light.intensity = 2.25f;
                LDL_Light.enabled = false;

                LDL.transform.SetParent(car.transform, true);
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
