using Harmony12;
using System.Collections.Generic;

namespace LocoLightsMod
{
    [HarmonyPatch(typeof(CabInputSteamExtra), "OnEnable")]
    internal class HeadLightSwitchPatch
    {
        static CabInputSteamExtra instance;
        static LocoLights dl;

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
                dl = PlayerManager.Car.GetComponentInChildren<LocoLights>(true);

            if (dl != null) { lightCtrl.SetValue(dl.isOn ? 1f : 0f); }

            lightCtrl.ValueChanged += (e =>
            {
                if (dl != null) { dl.ToggleHeadLight(PlayerManager.Car); }
            });
        }
    }
}