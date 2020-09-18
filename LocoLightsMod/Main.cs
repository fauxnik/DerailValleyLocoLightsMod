using Harmony12;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace LocoLightsMod
{
#if DEBUG
    [EnableReloading]
#endif
    public class Main
    {
        public static Settings settings;
        public static Dictionary<string, GameObject> assets = new Dictionary<string, GameObject>();
        private static HarmonyInstance harmony;

        private static bool OnLoad(UnityModManager.ModEntry modEntry)
        {
            try { settings = Settings.Load<Settings>(modEntry); } catch { }

            modEntry.OnUnload = OnUnload;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;

            harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            LoadAssets(modEntry.Path);

            var trainCars = GameObject.FindObjectsOfType<TrainCar>();
            foreach (var car in trainCars)
            {
                TrainCar_Start_Patch.DoCreate(car);
            }

            return true;
        }

        private static bool OnUnload(UnityModManager.ModEntry modEntry)
        {
            var trainCars = GameObject.FindObjectsOfType<TrainCar>();
            foreach (var car in trainCars)
            {
                TrainCar_OnDestroy_Patch.DoDestroy(car);
            }

            harmony.UnpatchAll(modEntry.Info.Id);

            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry) { settings.Draw(modEntry); }
        static void OnSaveGUI(UnityModManager.ModEntry modEntry) { settings.Save(modEntry); }

        public static void Log(object message)
        {
#if DEBUG
            Debug.Log($"[LocoLights] >>> {message}");
#endif
        }

        public static void LogWarning(object message)
        {
            Debug.LogWarning($"[LocoLights] >>> {message}");
        }

        public static void LogError(object message)
        {
            Debug.LogError($"[LocoLights] >>> {message}");
        }

        private static void LoadAssets(string modPath)
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(modPath + "Resources/locolights");

            assets.Add("SH282_Tender_Light_Body", bundle.LoadAsset<GameObject>("Assets/Models/sh282-tender-light.prefab"));

            bundle.Unload(false);
        }
    }
}
