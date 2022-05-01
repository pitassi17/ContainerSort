

// SmartContainers.Inventory_Patch
using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using Logger = Jotunn.Logger;


namespace ContainerSort
{
	[HarmonyPatch(typeof(Inventory), "AddItem", new Type[] { typeof(ItemDrop.ItemData) })]
	public static class Inventory_Patch
	{
		public static void Postfix(Inventory __instance, ItemDrop.ItemData item, ref bool __result)
		{
			Logger.LogWarning("Inventory 1");

			if (!Mod.modEnabled.Value 
				|| __instance == null 
				|| Player.m_localPlayer == null 
				|| Player.m_localPlayer.IsTeleporting() 
				|| __instance.Equals(Player.m_localPlayer.GetInventory())
				|| ContainersTracker.isRearrangingItem)
			{
				Logger.LogWarning("Inventory Return 1");
				return;
			}
			if (InventoryGui.instance == null || !InventoryGui.instance.IsContainerOpen())
			{
				Logger.LogWarning("Inventory Return 2");
				return;
			}
			ContainersTracker.isRearrangingItem = true;
			Logger.LogWarning("Inventory Result: " + __result.ToString());

			if (!__result)
			{
				Logger.LogWarning("Inventory Rearrange 1");

				if (TryAddToNearBy(item))
				{
					Logger.LogWarning("Inventory Rearrange 2");

					__result = true;
					//Mod.PlayEffect(Mod.audioFeedbackEnabled.get_Value(), "sfx_lootspawn", Player.m_localPlayer.GetCenterPoint());
				}
			}
			else if (__instance.CountItems(item.m_shared.m_name) == item.m_stack && TryAddToNearBy(item))
			{
				Logger.LogWarning("Inventory Remove 1");

				__instance.RemoveItem(item);
				//Mod.PlayEffect(Mod.audioFeedbackEnabled.get_Value(), "sfx_lootspawn", Player.m_localPlayer.GetCenterPoint());
			}
			Logger.LogWarning("Inventory CountItems: " + __instance.CountItems(item.m_shared.m_name.ToString()));
			Logger.LogWarning("Inventory Stack: " + item.m_stack.ToString());

			ContainersTracker.isRearrangingItem = false;
			Logger.LogWarning("Inventory Done 1");
		}

		public static bool TryAddToNearBy(ItemDrop.ItemData item, bool allowCurrent = false, bool forceGrouping = false)
		{
			try
			{
				Logger.LogWarning("Nearby 1");

				Container openedContainer = InventoryGui.instance.m_currentContainer;
				List<Container> list = (from c in ContainersTracker.GetNearbyContainers((Player.m_localPlayer).transform.position)
										where openedContainer.GetHashCode() == c.GetHashCode() 
											|| c.m_nview.GetZDO().GetInt("InUse") == 0
										select c).ToList();
				Logger.LogWarning("Nearby 2");

				foreach (Container container in list)
				{
					Logger.LogWarning("Nearby Loop 1");

					if (container.GetInventory().CanAddItem(item) && (allowCurrent || openedContainer.GetHashCode() != container.GetHashCode() && container.GetInventory().HaveItem(item.m_shared.m_name)))
					{
						Logger.LogWarning("Nearby Loop 2");

						Console.instance.Print(Localization.instance.Localize(item.m_shared.m_name) + " routed because target container has same item stack");
						return AddItemToContainer(item, container, allowCurrent);
					}
				}
			}
			catch (Exception ex)
			{
				Mod.log.LogError((object)("Failed to rearrange container items: " + ex));
			}
			return false;
		}

		private static bool AddItemToContainer(ItemDrop.ItemData item, Container targetContainer, bool allowCurrent = false)
		{
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			Container currentContainer = InventoryGui.instance.m_currentContainer;
			if (allowCurrent || ((object)currentContainer).GetHashCode() != ((object)targetContainer).GetHashCode())
			{
				bool num = targetContainer.GetInventory().AddItem(item);
				if (num)
				{
					targetContainer.Save();
					targetContainer.GetInventory().Changed();
					if (Mod.hudMessageEnabled.Value && !InventoryGui.instance.m_currentContainer.Equals(targetContainer))
					{
						MessageHud.instance.QueueUnlockMsg(item.GetIcon(), item.m_shared.m_name, Mod.hudMessageText.Value);
					}
					//Mod.PlayEffect(Mod.effectFeedbackEnabled.get_Value(), "vfx_Potion_health_medium", ((Component)targetContainer).get_transform().get_position());
				}
				return num;
			}
			return false;
		}
	}
}