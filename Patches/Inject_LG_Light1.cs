using UnityEngine;
using HarmonyLib;
using LevelGeneration;
using EOSExt.EMP.Impl;
using EOSExt.EMP.Impl.Handlers;
using ExtraObjectiveSetup.Utils;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System.Collections.Generic;
using System;
using System.Reflection;
//using AWO.WEE.Replicators;

namespace EOSExt.EMP.Patches
{
    [HarmonyPatch]
    internal static class Inject_LG_Light
    {
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(LG_Light), nameof(LG_Light.Start))]
        internal static void Pre_Start(LG_Light __instance) => __instance.gameObject.AddComponent<EMPController>().AssignHandler(new EMPLightHandler());

        //[HarmonyPostfix]
        //[HarmonyAfter([AWOUtil.PLUGIN_GUID])]
        //[HarmonyPatch(typeof(LG_BuildZoneLightsJob), nameof(LG_BuildZoneLightsJob.Build))]
        //private static void Post_ApplyColorScheme(bool __result, LG_BuildZoneLightsJob __instance)
        //{
        //    if (!__result)
        //        return;

        //    var zone = __instance.m_zone;
        //    if (zone == null)
        //        return;

        //    var lightsInZone = new List<LG_Light>();
        //    foreach (var nodes in zone.m_courseNodes)
        //    {
        //        lightsInZone.AddRange(nodes.m_area.GetComponentsInChildren<LG_Light>(false));
        //    }

        //    if(AWOUtil.IsLoaded)
        //    {
        //        var zoneReplicator = zone.gameObject.GetComponent<ZoneLightReplicator>();
        //        if (zoneReplicator != null)
        //        {
        //            zoneReplicator.Replicator.OnStateChanged += (oldState, newState, isRecall) => {
        //                foreach (var l in zoneReplicator.LightsInZone)
        //                {
        //                    var light = l.Light;
        //                    var handler = EMPLightHandler.GetHandler(light);
        //                    if (handler != null)
        //                    {
        //                        handler.SetOriginalColor(light.m_color);
        //                        handler.SetOriginalIntensity(light.m_intensity);
        //                    }
        //                }
        //            };
        //        }
        //    }

        //    foreach (var light in lightsInZone)
        //    {
        //        var handler = EMPLightHandler.GetHandler(light);
        //        if (handler != null)
        //        {
        //            handler.SetOriginalColor(light.m_color);
        //            handler.SetOriginalIntensity(light.m_intensity);
        //        }
        //    }
        //}

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
                    if (handler != null)
                    {
                        handler.SetOriginalColor(light.m_color);
                        handler.SetOriginalIntensity(light.m_intensity);
                    }
                }
            }
        }
    }
}
