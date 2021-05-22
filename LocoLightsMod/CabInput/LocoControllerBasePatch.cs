using Harmony12;
using System.Collections.Generic;
using UnityEngine;

namespace LocoLightsMod
{
	[HarmonyPatch(typeof(LocoControllerBase), "SetReverser")]
	class LocoControllerBase_SetReverser_Patch
	{
		static void Postfix(LocoControllerBase __instance)
		{
			// use switch on fireman's side of cab instead of reverser for steam loco
			if (__instance.train.carType == TrainCarType.LocoSteamHeavy) return;

			LocoLights locoLights = __instance.train.transform.gameObject.GetComponent<LocoLights>();
			if (locoLights == null) { return; }

			locoLights.SetDirection(__instance.reverser);
		}
	}
}
