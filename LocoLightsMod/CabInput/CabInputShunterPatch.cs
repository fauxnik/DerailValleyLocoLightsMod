using Harmony12;
using System.Collections.Generic;
using UnityEngine;

namespace LocoLightsMod
{
    [HarmonyPatch(typeof(CabInputShunter), "OnEnable")]
    internal class DE2HeadLightSwitchPatch
    {
        static CabInputShunter instance;
        static LocoLights locoLights;

        private static void Postfix(CabInputShunter __instance)
        {
            instance = __instance;
            instance.StartCoroutine(AttachListeners());
        }

        static IEnumerator<object> AttachListeners()
        {
            yield return (object)null;

            var rotary02 = GameObjectUtils.FindObject(instance.transform.gameObject, "C headlights dash_rotary02");
            DV.CabControls.ControlImplBase lightCtrl = rotary02.gameObject.GetComponent<DV.CabControls.ControlImplBase>();

            if (PlayerManager.Car.carType == TrainCarType.LocoShunter)
                locoLights = PlayerManager.Car.GetComponent<LocoLights>();

            if (locoLights != null) { lightCtrl.SetValue(locoLights.IsOn ? 1f : 0f); }

            lightCtrl.ValueChanged += (e =>
            {
                if (locoLights != null) { locoLights.SetLights(e.newValue > 0.5f); }
            });
        }
    }
}