using HarmonyLib;
using EOSExt.EMP.Impl;
using EOSExt.EMP.Impl.Handlers;

namespace EOSExt.EMP.Patches
{
    [HarmonyPatch]
    internal static class Inject_PlayerHUD
    {
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch((typeof(PlayerGuiLayer)), nameof(PlayerGuiLayer.UpdateGUIElementsVisibility))]
        private static bool Pre_UpdateGUIElementsVisibility()
        {
            if (EMPPlayerHudHandler.Instance != null && EMPPlayerHudHandler.Instance.IsEMPed())
            {
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch((typeof(CellSettingsApply)), nameof(CellSettingsApply.ApplyPlayerGhostOpacity))]
        private static void Pre_ApplyPlayerGhostOpacity(ref float value) 
        {
            if (EMPPlayerHudHandler.Instance != null && EMPPlayerHudHandler.Instance.IsEMPed())
            {
                value = 0.0f;
            }
        }

        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch((typeof(CellSettingsApply)), nameof(CellSettingsApply.ApplyHUDAlwaysShowTeammateInfo))]
        private static void Pre_ApplyHUDAlwaysShowTeammateInfo(ref bool value)
        {
            if (EMPPlayerHudHandler.Instance != null && EMPPlayerHudHandler.Instance.IsEMPed())
            {
                value = false;
            }
        }
    }
}
