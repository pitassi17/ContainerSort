using HarmonyLib;

namespace ContainerSort
{
	[HarmonyPatch(typeof(Container), "OnDestroyed")]
	internal static class Container_OnDestroyed_Patch
	{
		private static void Prefix(Container __instance)
		{
			if (Mod.modEnabled.Value)
			{
				ContainersTracker.containerList.Remove(__instance);
			}
		}
	}
}
