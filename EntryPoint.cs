using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;


namespace EOSExt.EMP
{
    [BepInDependency("dev.gtfomodding.gtfo-api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("GTFO.FloLib", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("Inas.ExtraObjectiveSetup", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(AWOUtil.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(AUTHOR + "." + PLUGIN_NAME, PLUGIN_NAME, VERSION)]
    
    public class EntryPoint: BasePlugin
    {
        public const string AUTHOR = "Inas";
        public const string PLUGIN_NAME = "EOSExt.EMP";
        public const string VERSION = "1.1.7";

        private Harmony m_Harmony;
        
        public override void Load()
        {
            m_Harmony = new Harmony("EOSExt.EMP");
            m_Harmony.PatchAll();

            SetupManagers();
        }

        /// <summary>
        /// Explicitly invoke Init() to all managers to eager-load, which in the meantime defines chained puzzle creation order if any
        /// </summary>
        private void SetupManagers()
        {
            EMPManager.Current.Init();
        }
    }
}

