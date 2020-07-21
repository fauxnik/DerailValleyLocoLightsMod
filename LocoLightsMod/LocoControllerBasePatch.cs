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
			Debug.Log($"[LocoLights] SetReverser patch (reverser={__instance.reverser})");
			LocoLights locoLights = __instance.train.transform.gameObject.GetComponent<LocoLights>();
			if (locoLights == null) { return; }

			locoLights.SetDirection(__instance.reverser);
		}
	}
}
