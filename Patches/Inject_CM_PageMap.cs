using CellMenu;
using HarmonyLib;

namespace EOSExt.EMP.Patches
{
    [HarmonyPatch]
    [HarmonyWrapSafe]
    internal class Inject_CM_PageMap
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CM_PageMap), nameof(CM_PageMap.UpdatePlayerData))]
        private static void Pre_CM_PageMap_OnEnable()
        {
            var map = MainMenuGuiLayer.Current.PageMap;
            if (map == null 
                || RundownManager.ActiveExpedition == null 
                || GameStateManager.CurrentStateName != eGameStateName.InLevel) return;

            if (EMPManager.Current.IsPlayerMapEMPD())
            {
                map.SetMapVisualsIsActive(false);
                map.SetMapDisconnetedTextIsActive(true);
            }
        }
    }
}
