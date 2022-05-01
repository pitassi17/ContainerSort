// ContainerSort
// a Valheim mod skeleton using Jötunn
// 
// File:    ContainerSort.cs
// Project: ContainerSort

using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
//using Jotunn.Entities;
//using Jotunn.Managers;

namespace ContainerSort
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    //[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    public class Mod : BaseUnityPlugin
    {
        public const string PluginGUID = "frolo.ContainerSort";
        public const string PluginName = "ContainerSort";
        public const string PluginVersion = "0.0.1";

        // Use this class to add your own localization to the game
        // https://valheim-modding.github.io/Jotunn/tutorials/localization.html
        //public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

        internal readonly Harmony harmony;
        internal readonly Assembly assembly;
        public static ConfigFile config;
        public static ManualLogSource log;
        public static ConfigEntry<int> range;
        public static ConfigEntry<bool> modEnabled;
        public static ConfigEntry<bool> hudMessageEnabled;
        public static ConfigEntry<string> hudMessageText;

        public static bool ignoreUpdate = false;

        public Mod()
        {
            harmony = new Harmony("frolo.ContainerSort");
            assembly = Assembly.GetExecutingAssembly();
            config = this.Config;
            log = this.Logger;
        }

        private void Awake()
        {
            // Jotunn comes with its own Logger class to provide a consistent Log style for all mods using it
            Jotunn.Logger.LogInfo("ContainerSort has landed");

            modEnabled = this.Config.Bind<bool>("General", "enabled", true, "Enables this mod");
            range = this.Config.Bind<int>("General", "range", 14, new ConfigDescription("Range within which containers will participate in resources arrangement.", new AcceptableValueRange<int>(5, 99), Array.Empty<object>()));
            hudMessageEnabled = this.Config.Bind<bool>("General", "hudMessageEnabled", true, "Enables hud message indicating that item was routed & placed to some other chest");
            hudMessageText = this.Config.Bind<string>("General", "hudMessageText", "Routed to another Chest", "HUD message consists of Icon, item-name plus this text.");

            ContainersTracker.Init();
            //this.Config.SettingChanged((EventHandler<SettingChangedEventArgs>)HandleConfigUpdate);

            harmony.PatchAll(assembly);
        }

        private void HandleConfigUpdate(object sender, SettingChangedEventArgs e)
        {
            if (ignoreUpdate)
            {
                ignoreUpdate = false;
                log.LogInfo($"SettingChangedEventArgs (ignored) {e.ChangedSetting.Definition}");
                return;
            }
        }

        public static void PlayEffect(bool allowed, string prefabName, Vector3 pos)
        {
            if (allowed)
            {
                GameObject prefab = ZNetScene.instance.GetPrefab(prefabName);
                if (prefab != null)
                {
                    UnityEngine.Object.Instantiate(prefab, pos, Quaternion.identity);
                }
                else
                {
                    log.LogWarning(("Failed to locate FeedbackEffect " + prefabName + " prefab"));
                }
            }
        }

        private void OnDestroy()
        {
            harmony.UnpatchSelf();
            this.Config.Reload();
            ContainersTracker.containerList.Clear();
        }
    }
}

