using HarmonyLib;
using UnityEngine;
using Logger = Jotunn.Logger;

namespace ContainerSort
{
	[HarmonyPatch(typeof(Container), "Awake")]
	internal static class Container_Awake_Patch
	{
		private static void Postfix(Container __instance)
		{
			Logger.LogWarning("Awake 1");
			if (Mod.modEnabled.Value && !__instance.GetHoverName().StartsWith("Treasure") && __instance.GetInventory() != null && __instance.m_nview.IsValid() && __instance.m_nview.GetZDO().GetLong("creator".GetStableHashCode(), 0L) != 0L)
			{
				Logger.LogWarning("Awake 2");
				ContainersTracker.containerList.Add(__instance);
			}
		}
	}
}
