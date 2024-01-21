using System;
using EOSExt.EMP.Definition;
using ExtraObjectiveSetup.Utils;
using FloLib.Networks.Replications;
using LevelGeneration;

namespace EOSExt.EMP.Impl
{
    public enum ActiveState
    {
        DISABLED,
        ENABLED,
    }

    public struct pEMPState
    {
        public ActiveState status;

        public pEMPState() { }

        public pEMPState(ActiveState status) { this.status = status; }

        public pEMPState(pEMPState o)
        {
            status = o.status;
        }
    }

    public class pEMP : EMPShock
    {
        public pEMPDefinition def { get; private set; }

        public StateReplicator<pEMPState> StateReplicator { get; private set; }

        public override ActiveState State => StateReplicator != null ? StateReplicator.State.status : ActiveState.DISABLED;

        public override float RemainingTime => State == ActiveState.ENABLED ? float.PositiveInfinity : float.NegativeInfinity;

        public override ItemToDisable ItemToDisable => def != null ? def.ItemToDisable : DISABLE_NOTHING;

        public void OnStateChanged(pEMPState oldState, pEMPState newState, bool isRecall)
        {
            if (def == null || oldState.status == newState.status) return;

            EOSLogger.Debug($"pEMP_{def.pEMPIndex} Change state: {oldState.status} -> {newState.status}");
            switch (newState.status)
            {
                case ActiveState.DISABLED:
                    endTime = float.NegativeInfinity; break;
                case ActiveState.ENABLED:
                    endTime = float.PositiveInfinity; break;
                default: throw new NotImplementedException();
            }

            foreach (EMPController empTarget in EMPManager.Current.EMPTargets)
            {
                if (empTarget.GetComponent<LG_Light>() == null) continue;
                switch (newState.status)
                {
                    case ActiveState.DISABLED:
                        empTarget.ClearTime(); // TODO: this would clear effect on instant shock 
                        break;
                    case ActiveState.ENABLED:
                        if (InRange(empTarget.Position))
                        {
                            empTarget.AddTime(float.PositiveInfinity);
                        }
                        else
                        {
                            empTarget.ClearTime(); // TODO: this would clear effect on instant shock 
                        }
                        break;
                    default: throw new NotImplementedException();
                }
            }
        }

        public void ChangeState(ActiveState newState) => StateReplicator?.SetState(new pEMPState() { status = newState });

        internal void SetupReplicator(uint replicatorID)
        {
            StateReplicator = StateReplicator<pEMPState>.Create(replicatorID, new pEMPState() { status = ActiveState.DISABLED }, LifeTimeType.Level);
            StateReplicator.OnStateChanged += OnStateChanged;
        }

        public pEMP(pEMPDefinition def)
            : base(def.Position.ToVector3(), def.Range, float.NegativeInfinity)
        {
            this.def = new(def);
        }

        internal void Destroy()
        {
            StateReplicator = null;
            def = null;
        }
    }
}
