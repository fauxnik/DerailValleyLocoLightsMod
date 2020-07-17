using Harmony12;
using System.Collections.Generic;

namespace LocoLightsMod
{
    [HarmonyPatch(typeof(TrainCar), "Start")]
    internal class HeadLightPatch
    {
        private static void Postfix(TrainCar __instance)
        {
            if (!Main.enabled) return;

            try
            {
                if (__instance.carType == TrainCarType.LocoSteamHeavy)
                    Main.CreateHeadLight(__instance);
            }
            catch { }
        }
    }

    [HarmonyPatch(typeof(CabInputSteamExtra), "OnEnable")]
    internal class HeadLightSwitchPatch
    {
        static CabInputSteamExtra instance;
        static HeadLight dl;

        private static void Postfix(CabInputSteamExtra __instance)
        {
            if (!Main.enabled) return;

            instance = __instance;
            instance.StartCoroutine(AttachListeners());
        }

        static IEnumerator<object> AttachListeners()
        {
            yield return (object)null;

            DV.CabControls.ControlImplBase lightCtrl = instance.transform.Find("C inidactor light switch").gameObject.GetComponent<DV.CabControls.ControlImplBase>();

            if (PlayerManager.Car.carType == TrainCarType.LocoSteamHeavy)
                dl = PlayerManager.Car.GetComponentInChildren<HeadLight>(true);

            if (dl != null) { lightCtrl.SetValue(dl.isOn ? 1f : 0f); }

            lightCtrl.ValueChanged += (e =>
            {
                if (dl != null) { dl.ToggleHeadLight(PlayerManager.Car); }
            });
        }
    }
}