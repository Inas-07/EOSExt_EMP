using System.Collections.Generic;
using ExtraObjectiveSetup.Utils;
using Player;
using GameData;
using Gear;
using EOSExt.EMP.Impl.Handlers;
using ExtraObjectiveSetup;
using ExtraObjectiveSetup.BaseClasses;
using EOSExt.EMP.Definition;
using GTFO.API;
using EOSExt.EMP.EMPComponent;
using EOSExt.EMP.Impl.PersistentEMP;

namespace EOSExt.EMP
{
    public partial class EMPManager: GenericExpeditionDefinitionManager<pEMPDefinition>
    {
        private Dictionary<uint, pEMP> _pEMPs { get; } = new();
        
        public IEnumerable<pEMP> pEMPs => _pEMPs.Values;

        protected override string DEFINITION_NAME => "PersistentEMP";

        public void TogglepEMPState(uint pEMPIndex, bool enabled)
        {
            if (!_pEMPs.ContainsKey(pEMPIndex))
            {
                EOSLogger.Error($"TogglepEMPState: pEMPIndex {pEMPIndex} not defined");
                return;
            }

            ActiveState newState = enabled ? ActiveState.ENABLED : ActiveState.DISABLED;
            var pEMP = _pEMPs[pEMPIndex];
            pEMP.ChangeState(newState);
        }

        public void TogglepEMPState(WardenObjectiveEventData e)
        {
            uint pEMPIndex = (uint)e.Count;
            bool enabled = e.Enabled;
            TogglepEMPState(pEMPIndex, enabled);
        }

        private void pEMPInit()
        {
            LevelAPI.OnBuildStart += () => { pEMPClear(); InitializepEMPs(); };
            LevelAPI.OnLevelCleanup += pEMPClear;
        }

        private void pEMPClear()
        {
            _pEMPs.Clear();
        }

        private void InitializepEMPs()
        {
            if (!definitions.ContainsKey(CurrentMainLevelLayout)) return;
            var expDef = definitions[CurrentMainLevelLayout];
            if (expDef == null || expDef.Definitions.Count < 1) return;

            foreach(var pEMPDef in expDef.Definitions)
            {
                var pEMP = new pEMP(pEMPDef);
                _pEMPs[pEMPDef.pEMPIndex] = pEMP;

                uint allotedID = EOSNetworking.AllotReplicatorID();
                if (allotedID == EOSNetworking.INVALID_ID)
                {
                    EOSLogger.Error("SetuppEMPReplicators: replicator ID depleted, cannot set up!");
                    continue;
                }

                pEMP.SetupReplicator(allotedID);
                EOSLogger.Debug($"pEMP_{pEMPDef.pEMPIndex} initialized");
            }
        }
    }
}
