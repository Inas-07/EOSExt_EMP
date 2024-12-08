//using UnityEngine;
//using HarmonyLib;
//using LevelGeneration;
//using EOSExt.EMP.Impl;
//using EOSExt.EMP.Impl.Handlers;
//using ExtraObjectiveSetup.Utils;
//using Il2CppInterop.Runtime.InteropTypes.Arrays;
//using System.Collections.Generic;
//using System;
//using System.Reflection;

//namespace EOSExt.EMP.Patches
//{
//    [HarmonyPatch]
//    internal static class Inject_LG_Light
//    {
//        [HarmonyPrefix]
//        [HarmonyWrapSafe]
//        [HarmonyPatch(typeof(LG_Light), nameof(LG_Light.Start))]
//        internal static void Pre_Start(LG_Light __instance) => __instance.gameObject.AddComponent<EMPController>().AssignHandler(new EMPLightHandler());

//        [HarmonyPostfix]
//        [HarmonyAfter(["GTFO.AWO"])]
//        [HarmonyPatch(typeof(LG_BuildZoneLightsJob), nameof(LG_BuildZoneLightsJob.Build))]
//        private static void Post_ApplyColorScheme(bool __result, LG_BuildZoneLightsJob __instance)
//        {
//            if (!__result)
//                return;

//            var zone = __instance.m_zone;
//            if (zone == null)
//                return;
            
//            var lightsInZone = new List<LG_Light>();
//            foreach (var nodes in zone.m_courseNodes)
//            {
//                lightsInZone.AddRange(nodes.m_area.GetComponentsInChildren<LG_Light>(false));
//            }

//            var zoneReplicator = zone.gameObject.GetComponent("AWO.WEE.Replicators.ZoneLightReplicator");
//            if (zoneReplicator != null)
//            {
//                var type = zoneReplicator.GetType();
//                object? replicator = type.GetProperty("Replicator")?.GetValue(zoneReplicator);
//                Type[]? stateTypes = replicator?.GetType().GetGenericArguments();
//                EventInfo? onStateChanged = replicator?.GetType()?.GetEvent("OnStateChanged");
//                if (onStateChanged != null)
//                {
//                    onStateChanged.AddEventHandler(zoneReplicator, )
//                }
//                // https://learn.microsoft.com/en-us/dotnet/api/system.reflection.eventinfo.addeventhandler?view=net-8.0
//            }

//            foreach (var light in lightsInZone)
//            {
//                var handler = EMPLightHandler.GetHandler(light);
//                if (handler != null)
//                {
//                    handler.SetOriginalColor(light.m_color);
//                    handler.SetOriginalIntensity(light.m_intensity);
//                }
//            }
//        }


//    }
//}
