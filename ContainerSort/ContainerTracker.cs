

// SmartContainers.ContainersTracker
using System.Collections.Generic;
using UnityEngine;
using Logger = Jotunn.Logger;

namespace ContainerSort
{
	public static class ContainersTracker
	{
		public static List<Container> containerList = new List<Container>();

		public static bool isRearrangingItem = false;

		public static List<Container> GetNearbyContainers(Vector3 center, bool all = false)
		{
			List<Container> list = new List<Container>();
			foreach (Container container in containerList)
			{
				int num = (all ? (Mod.range.Value * 2) : Mod.range.Value);
				if (container != null 
					&& container.transform != null 
					&& container.GetInventory() != null 
					&& container.GetComponentInParent<Piece>() != null 
					&& (num <= 0 || Vector3.Distance(center, container.transform.position) < (float)num) 
					&& container.CheckAccess(Player.m_localPlayer.GetPlayerID()))
				{
					list.Add(container);
				}
			}
			foreach (Container container in list)
			{
				Logger.LogWarning("Tracker Output: 1");
			}
			return list;
		}

		public static void Init()
		{
			Container[] array = Object.FindObjectsOfType<Container>();
			Mod.log.LogDebug($"tracking {array.Length} discovered containers.");
			Container[] array2 = array;
			foreach (Container container in array2)
			{
				if (!container.GetHoverName().StartsWith("Treasure") 
					&& container.GetInventory() != null 
					&& container.m_nview.IsValid() 
					&& container.m_nview.GetZDO().GetLong("creator".GetStableHashCode(), 0L) != 0L)
				{
					containerList.Add(container);
				}
			}
		}
	}
}

