using BepInEx.Unity.IL2CPP;
using ExtraObjectiveSetup.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EOSExt.EMP
{
    internal static class AWOUtil
    {
        public const string PLUGIN_GUID = "GTFO.AWO";

        public static bool IsLoaded { get; private set; } = false;

        static AWOUtil()
        {
            IsLoaded = IL2CPPChainloader.Instance.Plugins.TryGetValue(PLUGIN_GUID, out var _);
            EOSLogger.Debug($"EOSExt.EMP: AWO Loaded? {IsLoaded}");
        }
    }
}
