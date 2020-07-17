using Harmony12;
using System.Collections.Generic;
using UnityEngine;

namespace LocoLightsMod
{
    [HarmonyPatch(typeof(CabInputDiesel), "OnEnable")]
    internal class DE6HeadLightSwitchPatch
    {
        static CabInputDiesel instance;
        static LocoLights locoLights;

        private static void Postfix(CabInputDiesel __instance)
        {
            if (!Main.enabled) return;

            instance = __instance;
            instance.StartCoroutine(AttachListeners());
        }

        static IEnumerator<object> AttachListeners()
        {
            yield return (object)null;
            Debug.Log("attach DE6");

            var transform = GameObjectUtils.FindObject(instance.transform.gameObject, "C headlights dash_rotary01");
            Debug.Log("transform is null? " + (transform == null));
            DV.CabControls.ControlImplBase lightCtrl = transform
                .gameObject.GetComponent<DV.CabControls.ControlImplBase>();

            if (PlayerManager.Car.carType == TrainCarType.LocoDiesel)
                locoLights = PlayerManager.Car.GetComponent<LocoLights>();

            if (locoLights != null) { lightCtrl.SetValue(locoLights.IsOn ? 1f : 0f); }

            lightCtrl.ValueChanged += (e =>
            {
                if (locoLights != null) { locoLights.ToggleLights(); }
            });
        }
    }
}