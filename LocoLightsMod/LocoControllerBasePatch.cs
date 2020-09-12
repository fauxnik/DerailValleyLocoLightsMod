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
			// TODO: use switch on fireman's side of cab instead or reverser for steam loco
			LocoLights locoLights = __instance.train.transform.gameObject.GetComponent<LocoLights>();
			if (locoLights == null) { return; }

			locoLights.SetDirection(__instance.reverser);
		}
	}
}
