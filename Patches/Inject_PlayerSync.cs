using HarmonyLib;
using Player;
using EOSExt.EMP.Impl;
using EOSExt.EMP.Impl.Handlers;

namespace EOSExt.EMP.Patches
{
    [HarmonyPatch(typeof(PlayerSync))]
    internal static class Inject_PlayerSync
    {
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(PlayerSync.WantsToSetFlashlightEnabled))]
        internal static void Pre_WantsToSetFlashlightEnabled(PlayerSync __instance, ref bool enable)
        {
            if (EMPPlayerFlashLightHandler.Instance != null 
                && EMPPlayerFlashLightHandler.Instance.IsEMPed())
            {
                enable = false;
            }
            //if (enable != __instance.m_agent.Inventory.FlashlightEnabled)
            //    return true;
            //__instance.WantsToSetFlashlightEnabled(!__instance.m_agent.Inventory.FlashlightEnabled);
            //return false;
        }
    }
}
