﻿using Harmony12;
using LocoLightsMod.LocoLightDefinitions;
using LocoLightsMod.TrainCarLightDefinitions;
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

                // locomotives
                    case TrainCarType.LocoSteamHeavy:
                        SH282LightDefinition.SetupEngine(car);
                        break;
                    case TrainCarType.LocoShunter:
                        DE2LightDefinition.SetupLights(car);
                        break;
                    case TrainCarType.LocoDiesel:
                        DE6LightDefinition.SetupLights(car);
                        break;

                // train cars
                    case TrainCarType.PassengerBlue:
                    case TrainCarType.PassengerGreen:
                    case TrainCarType.PassengerRed:
                        PassengerCarLightDefinition.SetupLights(car);
                        break;
                    case TrainCarType.CabooseRed:
                        CabooseLightDefinitions.SetupLights(car);
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
                var tcLights = car.gameObject.GetComponent<TrainCarLights>();
                if (tcLights == null) { return; }

                if (car.carType == TrainCarType.LocoSteamHeavy) { SH282LightDefinition.TeardownTender(car.rearCoupler?.coupledTo?.train, car); }

                Main.Log($"Destroying LocoLights for train car {car.ID}");
                GameObject.Destroy(tcLights);
            }
            catch (Exception e) { Main.LogError(e); }
        }
    }
}
