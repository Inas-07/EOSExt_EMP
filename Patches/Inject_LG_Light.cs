using HarmonyLib;
using LevelGeneration;
using EOSExt.EMP.Impl;
using EOSExt.EMP.Impl.Handlers;

namespace EOSExt.EMP.Patches
{
    [HarmonyPatch(typeof(LG_Light))]
    internal static class Inject_LG_Light
    {
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(LG_Light.Start))]
        internal static void Pre_Start(LG_Light __instance) => __instance.gameObject.AddComponent<EMPController>().AssignHandler(new EMPLightHandler());
    }
}
