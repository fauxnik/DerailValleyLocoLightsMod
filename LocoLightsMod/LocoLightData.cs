namespace LocoLightsMod
{
	public struct LocoLightData
	{
		public readonly LocoLightType type;
		public readonly float min;
		public readonly float max;
		public readonly float scaleFwd;
		public readonly float scaleRev;
		public readonly float scaleNtl;
		public readonly float rate;

		public LocoLightData(
			LocoLightType type,
			float rate,
			float brightnessMin,
			float brightnessMax,
			float scaleFwd = 1f,
			float scaleRev = 1f,
			float scaleNtl = 1f)
		{
			this.type = type;
			this.min = brightnessMin;
			this.max = brightnessMax;
			this.scaleFwd = scaleFwd;
			this.scaleRev = scaleRev;
			this.scaleNtl = scaleNtl;
			this.rate = rate;
		}
	}
}
