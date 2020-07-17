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

            var rotary02 = GameObjectUtils.FindObject(instance.transform.gameObject, "C headlights dash_rotary02");
            DV.CabControls.ControlImplBase lightCtrl = rotary02.gameObject.GetComponent<DV.CabControls.ControlImplBase>();
            var reverser = GameObjectUtils.FindObject(instance.transform.gameObject, "C reverser");
            DV.CabControls.ControlImplBase revCtrl = reverser.gameObject.GetComponent<DV.CabControls.ControlImplBase>();

            if (PlayerManager.Car.carType == TrainCarType.LocoShunter)
                locoLights = PlayerManager.Car.GetComponent<LocoLights>();

            if (locoLights != null) { lightCtrl.SetValue(locoLights.IsOn ? 1f : 0f); }

            lightCtrl.ValueChanged += (e =>
            {
                if (locoLights != null) { locoLights.SetLights(e.newValue); }
            });

            // a bogus action is fired when the player exits and re-enters loco
            // acts like the reverser is set to 0.5 even when it's not
            // should be fine to skip the first event and let LocoLights.Init handle the initial value of 0.5
            bool isFirst = true;
            revCtrl.ValueChanged += (e =>
            {
                if (locoLights != null && !isFirst) { locoLights.SetDirection(e.newValue); }
                else { isFirst = false; }
            });
        }
    }
}