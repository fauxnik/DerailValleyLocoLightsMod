using Harmony12;
using System.Collections.Generic;
using UnityEngine;

namespace LocoLightsMod
{
    [HarmonyPatch(typeof(CabInputShunter), "OnEnable")]
    internal class DE2HeadLightSwitchPatch
    {
        private static void Postfix(CabInputShunter __instance)
        {
            __instance.StartCoroutine(AttachListeners(__instance));
        }

        static IEnumerator<object> AttachListeners(CabInputShunter instance)
        {
            yield return (object)null;

            var rotary02 = GameObjectUtils.FindObject(instance.transform.gameObject, "C headlights dash_rotary02");
            DV.CabControls.ControlImplBase lightCtrl = rotary02.gameObject.GetComponent<DV.CabControls.ControlImplBase>();

            LocoLights locoLights = TrainCar.Resolve(instance.gameObject).GetComponent<LocoLights>();
            if (locoLights != null) { lightCtrl.SetValue(locoLights.IsOn ? 1f : 0f); }

            lightCtrl.ValueChanged += (e =>
            {
                if (locoLights != null) { locoLights.SetLights(e.newValue > 0.5f); }
            });
        }
    }
}