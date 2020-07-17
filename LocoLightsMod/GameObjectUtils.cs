using UnityEngine;

namespace LocoLightsMod
{
	class GameObjectUtils
	{
		public static GameObject FindObject(GameObject parent, string name)
		{
			foreach(var transform in parent.GetComponentsInChildren<Transform>(true))
			{
				if (transform.name == name) { return transform.gameObject; }
			}
			return null;
		}
	}
}
