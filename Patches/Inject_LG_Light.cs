using UnityEngine;
using HarmonyLib;
using LevelGeneration;
using EOSExt.EMP.Impl;
using EOSExt.EMP.Impl.Handlers;
using ExtraObjectiveSetup.Utils;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace EOSExt.EMP.Patches
{
    [HarmonyPatch]
    internal static class Inject_LG_Light
    {
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(LG_Light), nameof(LG_Light.Start))]
        internal static void Pre_Start(LG_Light __instance) => __instance.gameObject.AddComponent<EMPController>().AssignHandler(new EMPLightHandler());

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LG_BuildZoneLightsJob), nameof(LG_BuildZoneLightsJob.ApplyLightSettings))]
        private static void Post_ApplyColorScheme(Il2CppReferenceArray<LG_Light> lights, bool firstPass)
        {
            foreach (var light in lights) 
            {
                if ((firstPass && light.gameObject.activeInHierarchy)
                    || (!firstPass && light.AvailableInLevel))
                {
                    var handler = EMPLightHandler.GetHandler(light);
                    if(handler != null)
                    {
                        handler.SetOriginalColor(light.m_color);
                        handler.SetOriginalIntensity(light.m_intensity);
                    }
                }
            }
        }
    }
}
