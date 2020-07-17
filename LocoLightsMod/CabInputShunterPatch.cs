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
            if (!Main.enabled) return;

            instance = __instance;
            instance.StartCoroutine(AttachListeners());
        }

        static IEnumerator<object> AttachListeners()
        {
            yield return (object)null;
            Debug.Log("attach DE2");

            var rotary02 = GameObjectUtils.FindObject(instance.transform.gameObject, "C headlights dash_rotary02");
            DV.CabControls.ControlImplBase lightCtrl = rotary02.gameObject.GetComponent<DV.CabControls.ControlImplBase>();
            var reverser = GameObjectUtils.FindObject(instance.transform.gameObject, "C reverser");
            DV.CabControls.ControlImplBase revCtrl = reverser.gameObject.GetComponent<DV.CabControls.ControlImplBase>();
            revCtrl.ValueChanged += (e =>
            {
                Debug.Log("shunter reverser: " + e.newValue);
            });

            if (PlayerManager.Car.carType == TrainCarType.LocoShunter)
                locoLights = PlayerManager.Car.GetComponent<LocoLights>();

            if (locoLights != null) { lightCtrl.SetValue(locoLights.IsOn ? 1f : 0f); }

            lightCtrl.ValueChanged += (e =>
            {
                if (locoLights != null) { locoLights.ToggleLights(); }
            });
        }
    }
}