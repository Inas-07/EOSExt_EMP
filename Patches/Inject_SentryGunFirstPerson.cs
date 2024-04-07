//using HarmonyLib;
//using EOSExt.EMP.Impl;
//using EOSExt.EMP.Impl.Handlers;
//using ExtraObjectiveSetup.Utils;

//namespace EOSExt.EMP.Patches
//{
//    [HarmonyPatch]
//    internal static class Inject_SentryGunFirstPerson
//    {
//        [HarmonyPrefix]
//        [HarmonyPatch(typeof(SentryGunFirstPerson), nameof(SentryGunFirstPerson.CheckCanPlace))]
//        internal static bool Pre_CheckCanPlace(SentryGunFirstPerson __instance, ref bool __result)
//        {
//            if(EMPSentryHandler.Instance == null)
//            {
//                EOSLogger.Error("No sentrygun handler fk you");
//                return true;
//            }

//            if (!EMPSentryHandler.Instance.IsEMPed())
//            {
//                return true;
//            }
//            else
//            {
//                __result = false;
//                return false;
//            }
//        }
//    }
//}
