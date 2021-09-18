using Harmony12;
using System.Collections.Generic;
using UnityEngine;

namespace LocoLightsMod
{
    [HarmonyPatch(typeof(CabInputDiesel), "OnEnable")]
    internal class DE6HeadLightSwitchPatch
    {
        private static void Postfix(CabInputDiesel __instance)
        {
            __instance.StartCoroutine(AttachListeners(__instance));
        }

        static IEnumerator<object> AttachListeners(CabInputDiesel instance)
        {
            yield return (object)null;

            var rotary01 = GameObjectUtils.FindObject(instance.transform.gameObject, "C headlights dash_rotary01");
            DV.CabControls.ControlImplBase lightCtrl = rotary01
                .gameObject.GetComponent<DV.CabControls.ControlImplBase>();

            LocoLights locoLights = TrainCar.Resolve(instance.gameObject).GetComponent<LocoLights>();
            if (locoLights != null) { lightCtrl.SetValue(locoLights.IsOn ? 1f : 0f); }

            lightCtrl.ValueChanged += (e =>
            {
                if (locoLights != null) { locoLights.SetLights(e.newValue > 0.5f); }
            });
        }
    }
}