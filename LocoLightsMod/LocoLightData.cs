using UnityEngine;

namespace LocoLightsMod
{
	public struct LocoLightData
	{
		public readonly string name;
		public readonly float min;
		public readonly float max;
		public readonly float fwd;
		public readonly float rev;
		public readonly float ntl;
		public readonly float rate;

		public LocoLightData(
			string name,
			float rate,
			float brightnessMin,
			float brightnessMax,
			float scalarFwd = 1f,
			float scalarRev = 1f,
			float scalarNtl = 1f)
		{
			this.name = name;
			this.min = brightnessMin;
			this.max = brightnessMax;
			this.fwd = Mathf.Clamp01(scalarFwd);
			this.rev = Mathf.Clamp01(scalarRev);
			this.ntl = Mathf.Clamp01(scalarNtl);
			this.rate = rate;
		}
	}
}
