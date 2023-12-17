using HarmonyLib;
using EOSExt.EMP.Impl;

namespace EOSExt.EMP.Patches
{
    [HarmonyPatch(typeof(SentryGunFirstPerson))]
    internal static class Inject_SentryGunFirstPerson
    {
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(SentryGunFirstPerson.CheckCanPlace))]
        internal static bool Pre_CheckCanPlace(ref bool __result)
        {
            if (!EMPHandler.IsLocalPlayerDisabled)
                return true;
            __result = false;
            return false;
        }
    }
}
