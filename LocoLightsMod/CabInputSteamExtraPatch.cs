using Harmony12;
using System.Collections.Generic;
using UnityEngine;

namespace LocoLightsMod
{
    [HarmonyPatch(typeof(CabInputSteamExtra), "OnEnable")]
    internal class SH282HeadLightSwitchPatch
    {
        static CabInputSteamExtra instance;
        static LocoLights locoLights;

        private static void Postfix(CabInputSteamExtra __instance)
        {
            if (!Main.enabled) return;

            instance = __instance;
            instance.StartCoroutine(AttachListeners());
        }

        static IEnumerator<object> AttachListeners()
        {
            yield return (object)null;
            Debug.Log("attach SH282");

            DV.CabControls.ControlImplBase lightCtrl = instance.transform.Find("C inidactor light switch")
                .gameObject.GetComponent<DV.CabControls.ControlImplBase>();

            if (PlayerManager.Car.carType == TrainCarType.LocoSteamHeavy)
                locoLights = PlayerManager.Car.GetComponent<LocoLights>();

            if (locoLights != null) { lightCtrl.SetValue(locoLights.IsOn ? 1f : 0f); }

            lightCtrl.ValueChanged += (e =>
            {
                if (locoLights != null) { locoLights.ToggleLights(); }
            });
        }
    }
}