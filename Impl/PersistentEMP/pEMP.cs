using System;
using EOSExt.EMP.Definition;
using EOSExt.EMP.Impl.Handlers;
using ExtraObjectiveSetup.Utils;
using FloLib.Networks.Replications;


namespace EOSExt.EMP.Impl.PersistentEMP
{

    public class pEMP : EMPShock
    {
        public pEMPDefinition def { get; private set; }

        public StateReplicator<pEMPState> StateReplicator { get; private set; }

        public override ActiveState State => StateReplicator != null ? StateReplicator.State.status : ActiveState.DISABLED;

        public override float RemainingTime => endTime;

        public override ItemToDisable ItemToDisable => def?.ItemToDisable ?? DISABLE_NOTHING;

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

            if (ItemToDisable.EnvLight == false) return;

            foreach (var h in EMPLightHandler.Instances)
            {
                switch (newState.status)
                {
                    case ActiveState.DISABLED:
                        h.RemoveAffectedBy(this);
                        break;
                    case ActiveState.ENABLED:
                        if (InRange(h.position))
                        {
                            h.AddAffectedBy(this);
                        }
                        else
                        {
                            h.RemoveAffectedBy(this);
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
            endTime = float.NegativeInfinity;
            StateReplicator = null;
            def = null;
        }

        public override bool Equals(object obj)
        {
            return obj is pEMP emp &&
                   base.Equals(obj) &&
                   position == emp.position &&
                   range == emp.range &&
                   def.Equals(emp.def);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), position, range, def);
        }
    }
}
