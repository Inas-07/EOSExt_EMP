using EOSExt.EMP.Impl.Handlers;
using HarmonyLib;
using Player;

namespace EOSExt.EMP.Patches
{
    [HarmonyPatch]
    internal static class Inject_PlayerAgent
    {
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(PlayerAgent), nameof(PlayerAgent.Setup))]
        internal static void Post_Setup(PlayerAgent __instance)
        {
            if (__instance.IsLocallyOwned)
            {
                EMPManager.Current.SetLocalPlayerAgent(__instance);
            }
        }
    }
}
