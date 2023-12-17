using HarmonyLib;
using EOSExt.EMP.Impl;
using EOSExt.EMP.Impl.Handlers;

namespace EOSExt.EMP.Patches
{
    [HarmonyPatch(typeof(SentryGunInstance))]
    internal static class Inject_SentryGunInstance
    {
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(SentryGunInstance.Setup))]
        internal static void Post_Setup(SentryGunInstance __instance)
        {
            __instance.gameObject.AddComponent<EMPController>().AssignHandler(new EMPSentryHandler());
        }
    }
}
