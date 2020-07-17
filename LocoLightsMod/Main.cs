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
                if (!enabled || !CarTypes.IsAnyLocomotiveOrTender(__instance.carType)) return;

                try
                {
                    LocoLights.SetupLights(__instance);
                }
                catch { }
            }
        }
    }
}
