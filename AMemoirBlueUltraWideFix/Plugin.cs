using BepInEx;

using HarmonyLib;

using System.Linq;

using UnityEngine;

namespace AMemoirBlueUltraWideFix
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            Harmony.CreateAndPatchAll(typeof(Patches));

            AMB_Service.resolutions = GetResolutions();
        }

        private RESOLUTION[] GetResolutions()
        {
            var list = AMB_Service.resolutions.ToList();
            Logger.LogInfo($"{Display.main.systemWidth} {Display.main.systemHeight} {Screen.resolutions[Screen.resolutions.Length - 1].refreshRate}");
            list.Add(new(Display.main.systemWidth, Display.main.systemHeight, Screen.resolutions[Screen.resolutions.Length - 1].refreshRate));
            return list.ToArray();
        }
    }

    [HarmonyPatch]
    public class Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Setting_Manager), nameof(Setting_Manager.CheckIfThisIsSupportedResolution))]
        public static bool OverrideSupportedResolutionCheck(ref bool __result)
        {
            __result = true;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FixRatio), nameof(FixRatio.RescaleCamera))]
        public static bool OverrideRescaleCamera(FixRatio __instance)
        {
            if(__instance.gameObject.tag == "UICamera")
            {
                __instance.gameObject.GetComponent<Camera>().aspect = 16f / 9f;
            }

            return __instance.gameObject.name != "Main Camera" && __instance.gameObject.tag != "UICamera";
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Letterbox_Manager), nameof(Letterbox_Manager.ShowLetterBox))]
        [HarmonyPatch(typeof(Letterbox_Manager), nameof(Letterbox_Manager.Start))]
        public static bool RemoveLetterboxing(Letterbox_Manager __instance)
        {
            __instance.RemoveLetterBox(0f);
            return false;
        }
    }
}