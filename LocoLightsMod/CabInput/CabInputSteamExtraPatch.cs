using Harmony12;
using System.Collections;
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
            instance = __instance;
            instance.StartCoroutine(AttachListeners());
        }

        private static float LeverPositionToDirection(float value)
        {
            return 1f - 2f * value;
        }

        private static float DirectionToLeverPosition(float direction)
        {
            return (1f - direction) / 2f;
        }

        static IEnumerator AttachListeners()
        {
            yield return null;

            var light_switch = GameObjectUtils.FindObject(instance.transform.gameObject, "C light lever");
            DV.CabControls.ControlImplBase lightCtrl = light_switch
                .gameObject.GetComponent<DV.CabControls.ControlImplBase>();

            if (PlayerManager.Car.carType == TrainCarType.LocoSteamHeavy)
                locoLights = PlayerManager.Car.GetComponent<LocoLights>();

            var isInitialized = Traverse.Create(lightCtrl).Field("isInitialized");

            Main.Log("pausing until Lever is initialized");
            while (!isInitialized.GetValue<bool>()) yield return null;
            Main.Log("Lever is initialized; resuming");

            if (locoLights != null)
            {
                float position = DirectionToLeverPosition(locoLights.IsOn ? locoLights.Direction : 0f);
                lightCtrl.SetValue(position);
            }

            lightCtrl.ValueChanged += (e =>
            {
                if (locoLights != null) {
                    float direction = LeverPositionToDirection(e.newValue);
                    locoLights.SetLights(Mathf.Abs(direction) > 0.25f);
                    locoLights.SetDirection(Mathf.Abs(direction) > 0.75f ? direction : 0f);
                }
            });
        }
    }
}