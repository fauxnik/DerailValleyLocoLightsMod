using Harmony12;
using LocoLightsMod.LocoLightDefinitions;
using System;
using UnityEngine;

namespace LocoLightsMod
{
    [HarmonyPatch(typeof(TrainCar), "Start")]
    internal class TrainCar_Start_Patch
    {
        private static void Postfix(TrainCar __instance)
        {
            DoCreate(__instance);
        }

        public static void DoCreate(TrainCar car)
        {
            try
            {
                switch (car.carType)
                {
                    default:
                        Main.Log($"Skipping LocoLights creation for train car {car.ID} ({car.carType})");
                        return;
                    case TrainCarType.LocoSteamHeavy:
                        SH282LightDefinition.SetupLights(car);
                        break;
                    case TrainCarType.LocoShunter:
                        DE2LightDefinition.SetupLights(car);
                        break;
                    case TrainCarType.LocoDiesel:
                        DE6LightDefinition.SetupLights(car);
                        break;
                }
                Main.Log($"Created LocoLights for train car {car.ID} ({car.carType})");
            }
            catch (Exception e) { Main.LogError(e); }
        }
    }

    [HarmonyPatch(typeof(TrainCar), "OnDestroy")]
    internal class TrainCar_OnDestroy_Patch
    {
        private static void Prefix(TrainCar __instance)
        {
            DoDestroy(__instance);
        }

        public static void DoDestroy(TrainCar car)
        {
            try
            {
                var locoLights = car.gameObject.GetComponent<LocoLights>();
                if (locoLights == null) { return; }

                Main.Log($"Destroying LocoLights for train car {car.ID}");
                GameObject.Destroy(locoLights);
            }
            catch (Exception e) { Main.LogError(e); }
        }
    }
}
