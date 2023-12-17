using HarmonyLib;
using EOSExt.EMP.Impl;
using EOSExt.EMP.Impl.Handlers;

namespace EOSExt.EMP.Patches
{
    [HarmonyPatch(typeof(PlayerGuiLayer))]
    internal static class Inject_PlayerGuiLayer
    {
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(PlayerGuiLayer.UpdateGUIElementsVisibility))]
        internal static bool Pre_UpdateGUIElementsVisibility()
        {
            if (EMPHandler.IsLocalPlayerDisabled)
            {
                foreach (var handler in EMPPlayerHudHandler.Handlers)
                {
                    handler.ForceState(EMPState.Off);
                }
                return false;
            }
            if (GameStateManager.CurrentStateName == eGameStateName.InLevel)
            {
                foreach (var handler in EMPPlayerHudHandler.Handlers)
                {
                    handler.ForceState(EMPState.On);
                }
            }
            return true;
        }
    }
}
