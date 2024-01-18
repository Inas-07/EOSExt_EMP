using System.Collections.Generic;
using ExtraObjectiveSetup.Utils;
using Player;
using GameData;
using Gear;
using EOSExt.EMP.Impl;
using EOSExt.EMP.Impl.Handlers;
using ExtraObjectiveSetup;
using ExtraObjectiveSetup.BaseClasses;
using EOSExt.EMP.Definition;
using GTFO.API;
using EOSExt.EMP.EMPComponent;

namespace EOSExt.EMP
{
    public partial class EMPManager: GenericExpeditionDefinitionManager<pEMPDefinition>
    {
        public struct pEMPState
        {
            public uint pEMPIndex;
            public bool enabled;
            public pEMPState() { }
            public pEMPState(pEMPState o) { o.pEMPIndex = pEMPIndex; o.enabled = enabled; }
        }

        private readonly Dictionary<uint, pEMP> _pEMPs = new();
        
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
                }
                else
                {
                    pEMP.SetupReplicator(allotedID);
                    EOSLogger.Debug($"SetuppEMPReplicators: replicator_{allotedID} setup! ");
                }
                EOSLogger.Warning($"pEMP_{pEMPDef.pEMPIndex} initialized");
            }
        }
    }
}
